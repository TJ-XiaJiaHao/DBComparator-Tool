<template>
  <div id="header">
    <dbinput></dbinput>
    <div class="col-sm-4"><label class="header-label header-db">{{ DBInfo }}</label></div>
    <div class="col-sm-4 compare-title"><label class="compare-title-text">{{ status }}</label></div>
    <div class="col-sm-4">
      <button type="submit" class="btn btn-default header-btns btn-recompare">RECOMPARE</button>
      <button type="submit" class="btn btn-default header-btns btn-reinput" @click="reinput">REINPUT</button>
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
        DBInfo: "",
        status: 'COMPARING...'
      }
    },
    methods: {
      compareDatabases: function (server1, dbname1, server2, dbname2) {
        this.status = "COMPARING...";
        var url = global.host + "/api/DBComparator?server1=" + server1 + "&dbname1=" + dbname1 + "&server2=" + server2 + "&dbname2=" + dbname2;
        console.log("[ GET ] - " + url);
        this.$http.get(url).then(function (response) {
            if (response.status == 200) {
              var data = response.body;
              if (data.code > 1000) {
                this.status = "ERROR:" + data.msg;
                $(".compare-title-text").css("color", "red");
              }
              else if (data.code == 1000) {
                this.status = "TABLES";
                console.log(data.tables);
              }
            }
          }, function (error) {
            console.log("[ ERROR ] - ", error);
          }
        );
      },
      showHeader: function () {
        $(".compare-title-text").css("color", "black");
        $("#header").animate({width: '100%'}, 500, function () {
          $(".header-db").animate({opacity: '1', paddingTop: '0'}, 1000);
          $(".header-btns").animate({opacity: '1', marginTop: '7px'}, 1000);
          $(".compare-title-text").animate({opacity: '1', marginTop: '0px'}, 1000);
          //showTable();
        });
      },
      hideHeader:function(){
        $(".header-db").css("opacity","0").css("paddingTop","10px");
        $(".header-btns").css("opacity","0").css("marginTop","17px");
        $(".compare-title-text").css("opacity","0").css("marginTop","10px");
        $("#header").animate({width:'0'},500);
        bus.$emit("showDBInput","");
      },
      reinput:function(){
        this.hideHeader();
      }
    }
    ,
    mounted()
    {
      var self = this;
      // 监听事件
      bus.$on("compareDB", function (data) {
        self.DBInfo = data.server1 + "," + data.dbname1 + " ; " + data.server2 + "," + data.dbname2;
        self.compareDatabases(data.server1, data.dbname1, data.server2, data.dbname2);
      });
      bus.$on("showHeader", function (data) {
        self.showHeader();
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
    text-align:left;
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
