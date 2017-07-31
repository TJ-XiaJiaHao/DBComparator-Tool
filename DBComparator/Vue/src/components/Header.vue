<template>
  <div id="header">
    <dbinput></dbinput>
    <div class="col-sm-5">
      <label class="header-label header-db">{{ server1 }}  {{ dbname1 }} <br/> {{ server2 }}  {{ dbname2 }}</label>
    </div>
    <div class="col-sm-2 compare-title"><label class="compare-title-text">{{ status }}</label></div>
    <div class="col-sm-5 btns">
      <div id="selector" class="header-btns">
        <select id="option-selector" class=" selectpicker form-control" data-width="100%" v-model="selected">
          <option v-for="(item,index) in options">{{ item }}</option>
        </select>
      </div>
      <button type="submit" class="btn btn-default header-btns btn-recompare" @click="recompare">RECOMPARE</button>
      <button type="submit" class="btn btn-default header-btns btn-reinput" @click="reinput">REINPUT</button>
      <button type="submit" class="btn btn-default header-btns btn-visualization" @click="visualization">CHARTS</button>
    </div>
  </div>
</template>

<script>
  import DBInput from '@/components/DBInput'
  import bus from "../assets/eventBus"
  export default {
    name: 'Header',
    components: {
      dbinput: DBInput
    },
    data () {
      return {
        server1: "",
        server2: "",
        username1: "",
        username2: "",
        password1: "",
        password2: "",
        dbname1: "",
        dbname2: "",
        status: 'COMPARING...',
        data: {},                                             // response data
        options: ["TABLES", "PROCEDURES", "FUNCTIONS"],
        selected: "0"
      }
    },
    methods: {

      /* Init some setting */
      clear: function () {
        this.status = "COMPARING...";
        this.data = {};
        $(".compare-title-text").css("color", "black");
        // console.log("[ EVENT ] - Header Clear");
      },

      /* Access the back-end to get the differences between two databases */
      getURL: function () {
        var url = global.host + "/api/DBComparator?";

        /* Connect to the local database */
        if (this.username1 == "" && this.password1 == "" && this.username2 == "" && this.password2 == "") {
          url += "server1=" + this.server1 + "&dbname1=" + this.dbname1 + "&server2=" + this.server2 + "&dbname2=" + this.dbname2;
        }

        /* Connect to remote database */
        else if (this.username1 != "" && this.password1 != "" && this.username2 != "" && this.password2 != "") {
          url += "server1=" + this.server1 + "&dbname1=" + this.dbname1 + "&username1=" + this.username1 + "&password1=" + this.password1 +
            "&server2=" + this.server2 + "&dbname2=" + this.dbname2 + "&username2=" + this.username2 + "&password2=" + this.password2;
        }

        /* Connect to remote and local databases */
        else if (this.username1 == "" && this.password1 == "" && this.username2 != "" && this.password2 != "") {
          url += "server1=" + this.server2 + "&dbname1=" + this.dbname2 + "&username1=" + this.username2 + "&password1=" + this.password2 +
            "&server2=" + this.server1 + "&dbname2=" + this.dbname1;
        }
        else if (this.username1 != "" && this.password1 != "" && this.username2 == "" && this.password2 == "") {
          url += "server1=" + this.server1 + "&dbname1=" + this.dbname1 + "&username1=" + this.username1 + "&password1=" + this.password1 +
            "&server2=" + this.server2 + "&dbname2=" + this.dbname2;
        }
        return url;
      },
      compareDatabases: function (callback) {
        // console.log("[ GET ] - " + url);
        this.clear();
        this.disableAll();
        var url = this.getURL();
        var self = this;
        this.$http.get(url).then(function (response) {
            if (response.status == 200) {
              if (response.body.code > 1000) {
                self.status = "ERROR:" + response.body.msg;
                $(".compare-title-text").css("color", "red");
              }
              else if (response.body.code == 1000) {
                self.status = "TABLES";
                self.data = response.body;
                console.log("[ RESPONSE ] - ", this.data);
                bus.$emit("changeData", this.data);
                self.selectedChange();
                if (callback != null) callback();
              }
            }
            else {
              self.status = "ERROR: Server error";
              $(".compare-title-text").css("color", "red");
              toastr.error("Server error");
            }
            self.enableAll();
          }, function (error) {
          self.status = "ERROR: Network error";
          $(".compare-title-text").css("color", "red");
            toastr.error("Network error!");
            self.enableAll();
            // console.log("[ ERROR ] - ", error);
          }
        );
      },

      /* Header IN adn OUT animation */
      showHeader: function () {
        var self = this;
        $("#header").animate({width: '100%'}, 500, function () {
          $(".header-db").animate({opacity: '1', paddingTop: '0'}, 1000);
          $(".header-btns").animate({opacity: '1', margin: '7px 10px 0'}, 1000);
          $(".compare-title-text").animate({opacity: '1', marginTop: '0px'}, 1000);
          bus.$emit("showTable", self.data);
        });
      },
      hideHeader: function () {
        bus.$emit("hideTable", "");
        $(".header-db").css("opacity", "0").css("paddingTop", "10px");
        $(".header-btns").css("opacity", "0").css("marginTop", "17px");
        $(".compare-title-text").css("opacity", "0").css("marginTop", "10px");
        $("#header").animate({width: '0'}, 500);
        bus.$emit("showDBInput", "");
      },

      /* Button events */
      reinput: function () {
        this.hideHeader();
      },
      recompare: function () {
        var self = this;
        this.compareDatabases(function () {
          bus.$emit("showTable", self.data);
        });
        bus.$emit("hideTable", "");
      },
      selectedChange: function () {
        switch (this.selected) {
          case "TABLES":
            this.status = "TABLES";
            bus.$emit("showTableDiff", "");
            break;
          case "PROCEDURES":
            this.status = "PROCEDURES";
            bus.$emit("showProcedureDiff");
            break;
          case "FUNCTIONS":
            this.status = "FUNCTIONS";
            bus.$emit("showFunctionDiff");
            break;
        }
      },
      disableAll: function () {
        $(".btn").attr("disabled", true);
      },
      enableAll: function () {
        $(".btn").attr("disabled", false);
      },
      visualization: function(){
        bus.$emit("visualization",this.data);
      }
    },
    watch: {
      selected: function (newSelected) {
        this.selectedChange();
      }
    },
    mounted()
    {
      var self = this;

      // Listen for events
      bus.$on("compareDB", function (data) {
        self.server1 = data.server1;
        self.server2 = data.server2;
        self.username1 = data.username1;
        self.username2 = data.username2;
        self.password1 = data.password1;
        self.password2 = data.password2;
        self.dbname1 = data.dbname1;
        self.dbname2 = data.dbname2;
        self.compareDatabases();
      });
      bus.$on("showHeader", function (data) {
        self.showHeader();
      });

      /* Init the selector */
      $(document).ready(function () {
        $('#option-selector').selectpicker({
          'selectedText': 'TABLES'
        });
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
    overflow: visible;
  }

  label {
    margin: 0;
  }

  /* 左边的文字　*/
  .header-label {
    height: 50px;
    line-height: 25px;
    opacity: 0;
    padding-top: 10px;
    text-align: left;
  }

  /*　右边的按钮　*/
  .btns{
    padding:0;
  }
  .btn-reinput, .btn-recompare, #selector,.btn-visualization {
    float: right;
    margin: 17px 5px 0;
    opacity: 0;
  }

  /* 中间的标题 */
  .compare-title {
    text-align: center;
    line-height: 50px;
  }

  .compare-title-text {
    opacity: 0;
    margin-top: 10px;
  }
</style>
