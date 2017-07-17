<template>
  <div id="header">
    <dbinput></dbinput>
    <div class="col-sm-4"><label class="header-label header-db"></label></div>
    <div class="col-sm-4 compare-title"><label class="compare-title-text">{{ status }}</label></div>
    <div class="col-sm-4">
      <button type="submit" class="btn btn-default header-btns btn-recompare">RECOMPARE</button>
      <button type="submit" class="btn btn-default header-btns btn-reinput">REINPUT</button>
    </div>
  </div>
</template>

<script>
  import DBInput from '@/components/DBInput'
  import bus from "../assets/eventBus"
  export default {
    name: 'DBInput',
    components: {
      dbinput: DBInput
    },
    data () {
      return {
        status: 'COMPARING...'
      }
    },
    methods: {
      compareDatabases: function (server1, dbname1, server2, dbname2) {
        var url = global.host + "/api/DBComparator?server1=" + server1 + "&dbname1=" + dbname1 + "&server2=" + server2 + "&dbname2=" + dbname2;
        console.log("[ GET ] - " + url);
        this.$http.get(url).then(function (data) {
          if (data.code > 1000) {
            this.status = "ERROR:" + data.msg;
            $(".compare-title-text")
          }
          else if (data.code == 1000) {
            $(".compare-title-text").text("TABLES");
            console.log(data.tables);
            initTableDiff(data.tables);
          }
        }, function (error) {
          console.log("[ ERROR ] - ", error);
        });
      }
    },
    mounted(){
      var self = this;
      // 监听事件
      bus.$on("compareDB", function (data) {
        self.compareDatabases(data.server1, data.dbname1, data.server2, data.dbname2);
      });
      bus.$on("showHeader", function (data) {
        this.status = "COMPARING...";
        $(".compare-title-text").css("color", "black");
      });
    }
  }
</script>

<style scoped>
  #header {
    width: 0;
    height: 50px;
    border-bottom: 1px solid;
    margin: 0 auto;
    overflow: hidden;
  }

  .header-label {
    height: 50px;
    line-height: 50px;
    opacity: 0;
    padding-top: 10px;
  }

  .btn-reinput, .btn-recompare {
    float: right;
    margin: 17px 10px;
    opacity: 0;
  }

  .compare-title {
    text-align: center;
    line-height: 50px;
  }

  .compare-title-text {
    opacity: 0;
    margin-top: 10px;
  }
</style>
