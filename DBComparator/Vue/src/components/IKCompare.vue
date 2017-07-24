<template>
  <div id="ik-compare">
    <div class="header">
      <div class="col-sm-2"></div>
      <div class="col-sm-2 dbname"><label>{{ dbname1 }}</label></div>
      <div class="col-sm-4"><label>{{ title }}</label></div>
      <div class="col-sm-2 dbname"><label>{{ dbname2 }}</label></div>
      <div class="col-sm-1">
        <input id="checkbox-all" type="checkbox" v-model="isAll">
        <label id="label-all" for="checkbox-all">ALL</label>
      </div>
      <div class="col-sm-1">
        <img class="btn-close" src="../assets/icon/down.png" @click="close"/>
      </div>
    </div>
    <div class="main">
      <div class="half left">
        <div v-for="(item, index) in data1">
          <div><label>{{ item.key }} :</label></div>
          <div>
            <div v-for="(val,index) in item.value">[ {{ val }} ] </div>
          </div>
        </div>
      </div>
      <div class="half right">
        <div v-for="(item, index) in data2">
          <div><label>{{ item.key }} :</label></div>
          <div>
            <div v-for="(val,index) in item.value">[ {{ val }} ] </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
  import bus from "../assets/eventBus"
  export default {
    name: 'DBInput',
    data () {
      return {
        isAll: false,
        dbname1: "dbname1",
        dbname2: "dbname2",
        title: "INDEXES",
        data1: [],
        data2: [],
        original: {
          type: "",
          data1: [],
          data2: []
        }
      }
    },
    methods: {
      showIK: function () {
        var height = $("#ik-compare").css("height");
        if (height == "0px") {
          $("#ik-compare").css("display", "block").animate({height: "50%"}, 500);
        }
      },
      showIndexes: function (indexes) {
        this.showIK();
        this.title = indexes[0].tbname;
        this.dbname1 = indexes[0].dbname;
        this.dbname2 = indexes[1].dbname;
        this.data1 = [];
        this.data2 = [];

        /* Initialize the data1 */
        this.data1.push({key: "indexes", value: indexes[0].indexes});
        this.original.data1 = this.data1;

        /* Initialize the data2 */
        this.data2.push({key: "indexes", value: indexes[1].indexes});
        this.original.data2 = this.data2;

        this.original.type = "INDEXES";
      },
      showKeys: function (keys) {
        this.showIK();
        this.title = keys[0].tbname;
        this.dbname1 = keys[0].dbname;
        this.dbname2 = keys[1].dbname;
        this.data1 = [];
        this.data2 = [];

        /* Initialize the data1 */
        this.data1.push({key: "primaryKeys", value: keys[0].primaryKeys});
        this.data1.push({key: "foreignKeys", value: keys[0].foreignKeys});
        this.original.data1 = this.data1;

        /* Initialize the data2 */
        this.data2.push({key: "primaryKeys", value: keys[1].primaryKeys});
        this.data2.push({key: "foreignKeys", value: keys[1].foreignKeys});
        this.original.data2 = this.data2;

        this.original.type = "KEYS";
      },
      toggleAll: function (flag) {
        if (flag) {
          this.data1 = this.original.data1;
          this.data2 = this.original.data2;
        }
        else {
          var self = this;
          var newData1 = [];
          var newData2 = [];

          /* Remove the coexist values from data1 */
          for (var i = 0; i < this.data1.length; i++) {
            newData1.push({key: this.data1[i].key, value: []});
            for (var j = 0; j < this.data1[i].value.length; j++) {
              if (this.data2[i].value.indexOf(this.data1[i].value[j]) < 0) {
                newData1[i].value.push(this.data1[i].value[j]);
              }
            }
          }

          /* Remove the coexist values from data2*/
          for (var i = 0; i < this.data2.length; i++) {
            newData2.push({key: this.data2[i].key, value: []});
            for (var j = 0; j < this.data2[i].value.length; j++) {
              if (this.data1[i].value.indexOf(this.data2[i].value[j]) < 0) {
                newData2[i].value.push(this.data2[i].value[j]);
              }
            }
          }

          this.data1 = newData1;
          this.data2 = newData2;
        }
      },
      close: function () {
        $("#ik-compare").animate({height: "0"}, 500, function () {
          $(this).css("display", "none");
        });
      }
    },
    watch: {
      isAll: function (newAll) {
        this.toggleAll(newAll);
      }
    },
    mounted() {
      var self = this;
      bus.$on("showIndexes", function (data) {
        // console.log("[ EVENT ] - showIndexes", data);
        self.showIndexes(data);
        self.toggleAll(self.isAll);
      });
      bus.$on("showKeys", function (data) {
        // console.log("[ EVENT ] - showKeys", data);
        self.showKeys(data);
        self.toggleAll(self.isAll);
      });
      bus.$on("hideIK", function (data) {
        self.close();
      })


      /* Change the scroll style */
      $(document).ready(function () {
        $(".half").niceScroll({
          styler: "fb",
          cursorcolor: "rgb(201,201,201)",
          cursorwidth: '0',
          cursorborderradius: '0',
          autohidemode: 'true',
          background: '#1B2426',
          spacebarenabled: false,
          cursorborder: '0'
        });
      });
    }
  }
</script>

<style scoped>

  label {
    margin: 0;
  }

  #ik-compare {
    position: fixed;
    bottom: 0;
    left: 0;
    width: 100%;
    height: 0;
    display: none;
    background: white;
    min-width: 800px;
  }

  .header {
    height: 40px;
    width: 100%;
    line-height: 40px;
    background: rgba(0, 0, 0, 0.2);
    color: white;
  }

  .btn-close {
    width: 15px;
    height: 15px;
    float: right;
    margin-top: 12px;
    cursor: pointer;
  }

  #checkbox-all {
    margin-top: 13px;
    float: right;
  }

  #label-all {
    float: right;
    margin-right: 5px;
  }

  .main {
    width: 100%;
    height: calc(100% - 40px);
  }

  .half {
    width: 50%;
    height: 100%;
    float: left;
    padding: 10px;
    text-align: left;
    overflow-y: scroll;
  }

  .left {
    border-right: 1px solid rgba(0, 0, 0, 0.2);
  }
</style>
