<template>
  <div id="column-compare">
    <div class="header">
      <div class="col-sm-4"></div>
      <div class="col-sm-4">
        <label class="title">COLUMNS</label>
      </div>
      <div class="col-sm-4">
        <button type="button" class="btn btn-default btn-close" @click="close">CLOSE</button>
      </div>
    </div>
    <div class="column-diff" v-for="(tb,tbIndex) in columns">
      <table class="table table-bordered">
        <thead>
        <tr>
          <th class="tb-head" v-for="(item,index) in header">
            {{ item }}
          </th>
        </tr>
        </thead>
        <tbody>
        <tr v-for="(row,rowIndex) in tb">
          <td v-if="rowIndex == 0" rowspan="2" class="column-name">{{ names[tbIndex] }}</td>
          <td v-for="(col,colIndex) in row"
              v-bind:class="{difference:tb[0][colIndex] != tb[1][colIndex] && colIndex > 0}">
            {{ col }}
          </td>
        </tr>
        </tbody>
      </table class="table table-bordered">
    </div>
  </div>
</template>

<script>
  import bus from "../assets/eventBus"
  export default {
    name: 'DBInput',
    data () {
      return {
        header: [                               // Table Headers
          "COLNAME",
          "DBNAME",
          "EXIST",
          "CHARACTER MAXIMUM LENGTH",
          "CHARACTER OCTET LENGTH",
          "COLUMN DEFAULT",
          "DATA_TYPE",
          "IS NULLABLE",
//          "NUMERIC PRECISION",
//          "NUMERIC PRECISION RADIX",
//          "NUMERIC SCALE"
        ],
        names: [],                              // Different Column Names
        columns: []                             // Table Rows Data
      }
    },
    methods: {

      /* Initialize the data and show the page */
      showColumn: function (columns) {
        this.names = [];
        this.columns = [];
        if (columns == null || columns.length == 0)return;

        /* Initialize the array for the table */
        for (var i = 0; i < columns.length; i++) {
          var db1 = columns[i].different[0];
          var db2 = columns[i].different[1];
          var row1 = [db1.dbname, db1.exist];
          var row2 = [db2.dbname, db2.exist];
          db1.propeties.sort(function (a, b) {
            return a.name > b.name;
          });
          for (var j = 0; j < 5; j++) {
            if (db1.propeties.length <= j || db1.propeties[j].value == "") row1.push("-");
            else row1.push(db1.propeties[j].value);
            if (db2.propeties.length <= j || db2.propeties[j].value == "") row2.push("-");
            else row2.push(db2.propeties[j].value);

          }
          this.names.push(columns[i].name);
          this.columns.push([row1, row2]);
        }

        /* Show the tables */
        $("#column-compare").css("display", "block").scrollTop(0).animate({height: '100%'}, 500);
      },

      /* Close the page, actually just set the page display to none*/
      close: function () {
        $("#column-compare").animate({height: '0'}, 500, function () {
          $(this).css("display", "none");
        });
      }
    },
    mounted() {
      var self = this;

      /* Listen for events */
      bus.$on("showColumn", function (data) {
        // console.log("[ EVENT ] - showColumn", data)
        self.showColumn(data);
      })

      /* Change the scroll style */
      $(document).ready(function () {
        $("#column-compare").niceScroll({
          styler: "fb",
          cursorcolor: "rgb(201,201,201)",
          cursorwidth: '8',
          cursorborderradius: '10px',
          background: 'rgba(0,0,0,0.1)',
          spacebarenabled: false,
          cursorborder: '0',
          zindex: '1000'
        });
      });
    }
  }
</script>

<style scoped>

  /* Header fixed at the top */
  .header {
    width: 100%;
    height: 50px;
    border-bottom: 1px solid;
    position: fixed;
    top: 0;
    left: 0;
    background: white;
  }

  .btn-close {
    float: right;
    margin: 10px 10px 0 0;
  }

  .title {
    text-align: left;
    line-height: 50px;
  }

  /* body */
  #column-compare {
    position: fixed;
    width: 100%;
    height: 0;
    top: 0;
    left: 0;
    display: none;
    background: white;
    z-index: 1000;
    overflow-y: scroll;
    overflow-x: hidden;
    padding-top: 60px;
  }

  /* table style */
  .column-diff {
    width: 100%;
    min-width: 800px;
    padding: 0 20px 0 20px;
  }

  .tb-head {
    text-align: center;
  }

  .column-name {
    text-align: center;
    line-height: 20px;
    padding-top:30px;
    color: deepskyblue;
    width:200px;
    max-width:200px;
    word-wrap:break-word;
  }

  .difference {
    color: red;
    font-weight: bold;
  }
</style>
