<template>
  <div id="ik-compare">
    <div class="header">
      <div class="col-sm-2"></div>
      <div class="col-sm-2 dbname"><label>{{ dbname1 }}</label></div>
      <div class="col-sm-4"><label>{{ title }}</label></div>
      <div class="col-sm-2 dbname"><label>{{ dbname2 }}</label></div>
      <div class="col-sm-2"><img class="btn-close" src="../assets/icon/down.png" @click="close"/></div>
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
        dbname1: "dbname1",
        dbname2: "dbname2",
        title: "INDEXES",
        data1: [],
        data2: [],
      }
    },
    methods: {
      showIndexes: function (indexes) {
        $("#ik-compare").css("display", "block").animate({height: "50%"}, 500);
        this.title = indexes[0].tbname;
        this.dbname1 = indexes[0].dbname;
        this.dbname2 = indexes[1].dbname;
        this.data1 = [];
        this.data2 = [];

        /* Initialize the data1 */
        this.data1.push({key: "indexes", value: indexes[0].indexes});

        /* Initialize the data2 */
        this.data2.push({key: "indexes", value: indexes[1].indexes});
      },
      showKeys: function (keys) {
        $("#ik-compare").css("display", "block").animate({height: "50%"}, 500);
        this.title = keys[0].tbname;
        this.dbname1 = keys[0].dbname;
        this.dbname2 = keys[1].dbname;
        this.data1 = [];
        this.data2 = [];

        /* Initialize the data1 */
        this.data1.push({key: "primaryKeys", value: keys[0].primaryKeys});
        this.data1.push({key: "foreignKeys", value: keys[0].foreignKeys});

        /* Initialize the data2 */
        this.data2.push({key: "primaryKeys", value: keys[1].primaryKeys});
        this.data2.push({key: "foreignKeys", value: keys[1].foreignKeys});
      },
      close: function () {
        $("#ik-compare").animate({height: "0"}, 500, function () {
          $(this).css("display", "none");
        });
      }
    },
    mounted() {
      var self = this;
      bus.$on("showIndexes", function (data) {
        console.log("[ EVENT ] - showIndexes", data);
        self.showIndexes(data);
      });
      bus.$on("showKeys", function (data) {
        console.log("[ EVENT ] - showKeys", data);
        self.showKeys(data);
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
