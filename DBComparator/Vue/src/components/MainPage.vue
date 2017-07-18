<template>
  <div id="main">
    <h4 v-if="isNoItems">no items are different</h4>
    <table class="table table-hover">
      <thead>
      <tr>
        <th class="tb-head" v-for="(item,index) in current.header" @click="sort(index)">{{ item }}</th>
      </tr>
      </thead>
      <tbody>
      <tr v-for="(row,rowIndex) in current.body">
        <td v-for="(col,colIndex) in row" v-bind:title="current.tooltip">{{ col }}</td>
      </tr>
      </tbody>
    </table>
  </div>
</template>

<script>
  import bus from "../assets/eventBus"
  export default {
    name: 'DBInput',
    data () {
      return {
        isNoItems: false,
        data: {},
        tableDiff: [],
        procedureDiff: [],
        functionDiff: [],
        current: {
          type: "TABLES",
          header: [],
          body: [],
          sortDir: -1,        // 1 means forward -1 means backward
          tooltip: ""
        }
      }
    },
    methods: {
      /* Table IN and OUT animation */
      showTable: function () {
        $("#main").animate({paddingLeft: '50px', opacity: '1'}, 500);
      },
      hideTable: function () {
        $("#main").animate({paddingLeft: '100px', opacity: '0'}, 0);
      },

      /* Clear all the data */
      clear: function () {
        this.tableDiff = [];
        this.procedureDiff = [];
        this.functionDiff = [];
        this.current.header = [];
        this.current.body = [];
      },

      /* Show different data depends on the show type*/
      initCurrent: function (headers, body, tooltip, type) {
        this.current.header = headers;
        this.current.body = body;
        this.current.tooltip = tooltip;
        this.current.type = type;
        if (this.current.body.lenght == 0) {
          this.isNoItems = true;
          this.current.header = [];
        }
        else this.isNoItems = false;
      },
      showTableDiff: function () {
        this.initCurrent(["NAME", "COEXIST", "BELONG", "COLUMNS", "INDEXES", "KEYS"], this.tableDiff,"", "TABLES")
      },
      showProcedureDiff: function () {
        this.initCurrent(["NAME", "COEXIST", "DBNAME", "EXIST", "DBNAME", "EXIST"], this.procedureDiff,"click to view details", "PROCEDURES")
      },
      showFunctionDiff: function () {
        this.initCurrent(["NAME", "COEXIST", "DBNAME", "EXIST", "DBNAME", "EXIST"], this.functionDiff,"click to view details", "FUNCTIONS")
      },

      /* Sort function, when you click the table header,table will be sorted by the alphabet order */
      sort: function (index) {
        var self = this;
        self.current.sortDir *= -1;
        this.current.body.sort(function (a, b) {
          return self.current.sortDir == 1 ? a[index] > b[index] : a[index] < b[index]
        })
      }
    },
    watch: {
      /* when data changed , refresh some relative data and refresh the front end*/
      data: function (newData) {
        var self = this;
        self.clear();
        if (newData.tables == null || newData.tables == undefined)return;

        /* Refresh different tables information */
        for (var i = 0; i < newData.tables.length; i++) {
          var item = newData.tables[i];
          self.tableDiff.push([item.name, item.coexist, item.dbnameIfNotCoexit, item.columns.length, item.indexes.length, item.keys.length]);
        }

        /* Refresh different procedures information */
        for (var i = 0; i < newData.storedProcedures.length; i++) {
          var item = newData.storedProcedures[i];
          self.procedureDiff.push([item.name, item.coexist, item.different[0].dbname, item.different[0].exist, item.different[1].dbname, item.different[1].exist]);
        }

        /* Refresh different function information */
        for (var i = 0; i < newData.functions.length; i++) {
          var item = newData.functions[i];
          self.functionDiff.push([item.name, item.coexist, item.different[0].dbname, item.different[0].exist, item.different[1].dbname, item.different[1].exist]);
        }

        /* Refresh the front end ,show different data depends on the show type*/
        switch (self.current.type) {
          case "TABLES":
            self.showTableDiff();
            break;
          case "PROCEDURES":
            self.showProcedureDiff();
            break;
          case "FUNCTIONS":
            self.showFunctionDiff();
            break;
          default:
            self.showTableDiff();
        }
      }
    },
    mounted() {
      var self = this;

      /* Listen for events*/
      bus.$on("showTable", function (data) {
        console.log("[ EVENT ] - showTable", data);
        self.showTable();
        self.data = data;
      })
      bus.$on("hideTable", function (data) {
        console.log("[ EVENT ] - hideTable", data);
        self.data = {}
        self.clear();
        self.hideTable();
      });
      bus.$on("changeData", function (data) {
        console.log("[ EVENT ] - changeData", data);
        self.data = data;
      });
      bus.$on("showTableDiff", function (data) {
        console.log("[ EVENT ] - showTableDiff");
        self.hideTable();
        self.showTable();
        self.showTableDiff();
      });
      bus.$on("showProcedureDiff", function (data) {
        console.log("[ EVENT ] - showProcedureDiff");
        self.hideTable();
        self.showTable();
        self.showProcedureDiff();
      });
      bus.$on("showFunctionDiff", function (data) {
        console.log("[ EVENT ] - showFunctionDiff");
        self.hideTable();
        self.showTable();
        self.showFunctionDiff();
      });
    }
  }
</script>

<style scoped>
  #main {
    width: 100%;
    height: calc(100% - 50px);
    padding: 20px 50px 10px 100px;
    opacity: 0;
    text-align: center;
  }

  .tb-head {
    text-align: center;
  }
</style>
