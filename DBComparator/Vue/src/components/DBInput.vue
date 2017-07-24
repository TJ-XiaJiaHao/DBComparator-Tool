<template>
  <div id="db-input">
    <div class="col-md-3 col-sm-2"></div>
    <div class="col-md-6 col-sm-8 db-input-div">
      <h1 class="db-input-title">DATABASE ONE</h1>
      <form class="form-horizontal db-input-form" role="form">
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="servername-one" placeholder="server name"
                   :title="tooltipmsg.servername" data-container="body" data-placement="right" v-model="server1">
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="db-name-one" placeholder="database name"
                   :title="tooltipmsg.dbname" data-container="body" data-placement="right" v-model="dbname1">
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-6">
            <input type="text" class="form-control input-lg db-input-text" id="db-username-one" placeholder="username"
                   :title="tooltipmsg.remote" data-container="body" data-placement="left" v-model="username1"/>
          </div>
          <div class="col-sm-6">
            <input type="password" class="form-control input-lg db-input-text" id="db-password-one" v-model="password1"
                   placeholder="password" :title="tooltipmsg.remote" data-container="body" data-placement="right"/>
          </div>
        </div>
      </form>
      <h1 class="db-input-title">DATABASE TWO</h1>
      <form class="form-horizontal db-input-form" role="form">
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="servername-two" placeholder="server name"
                   :title="tooltipmsg.servername" data-container="body" data-placement="right" v-model="server2">
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="db-name-two" placeholder="database name"
                   :title="tooltipmsg.dbname" data-container="body" data-placement="right" v-model="dbname2">
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-6">
            <input type="text" class="form-control input-lg db-input-text" id="db-username-two" placeholder="username"
                   :title="tooltipmsg.remote" data-container="body" data-placement="left" v-model="username2"/>
          </div>
          <div class="col-sm-6">
            <input type="password" class="form-control input-lg db-input-text" id="db-password-two" v-model="password2"
                   placeholder="password" :title="tooltipmsg.remote" data-container="body" data-placement="right"/>
          </div>
        </div>
      </form>
      <div class="col-sm-12 btn-compare-div">
        <button type="submit" class="btn btn-default btn-compare" @click="submit">Compare</button>
      </div>
    </div>
  </div>
</template>

<script>
  import bus from '../assets/eventBus'
  export default {
    name: 'DBInput',
    data () {
      return {
        tooltipmsg: {
          servername: "please input the server name you used in the sql server management tool !",
          dbname: "please input the database name you want to compare !",
          remote: "don't need if you connect to the local sql server with windows authentication !"
        },
        server1: "",
        dbname1: "",
        username1: "",
        password1: "",
        server2: "",
        dbname2: "",
        username2: "",
        password2: ""
      }
    },
    methods: {

      /* Check if the databases information equals null or "" */
      check: function () {
        if (this.server1 == "" || this.server2 == "" || this.dbname1 == "" || this.dbname2 == "") {
          toastr.error("server name and database name should not be empty !")
          return false;
        }
        if ((this.username1 == "" && this.password1 != "") || (this.username1 != "" && this.password1 == "")) {
          toastr.error("username and password should all be empty or all be not empty !");
          return;
        }
        if ((this.username2 == "" && this.password2 != "") || (this.username2 != "" && this.password2 == "")) {
          toastr.error("username and password should all be empty or all be not empty !");
          return;
        }
        return true;
      },

      /* Submit the databases information to header component */
      submit: function () {
        // console.log("[ SUBMIT ] - db1: " + this.server1 + " " + this.dbname1 + " ; db2: " + this.server2 + " " + this.dbname2);
        if (!this.check()) return;
        bus.$emit("compareDB", {
          server1: this.server1,
          dbname1: this.dbname1,
          username1: this.username1,
          password1: this.password1,
          server2: this.server2,
          dbname2: this.dbname2,
          username2: this.username2,
          password2: this.password2
        });
        this.hideDBInput();
      },

      /* DBInput IN and OUT animation*/
      hideDBInput: function () {
        var dbInput = $("#db-input");
        dbInput.animate({paddingTop: '30px', opacity: 0}, 500, function () {
          dbInput.css("display", "none");
          bus.$emit("showHeader", "COMPARING...");
        })
      },
      showDBInput: function (callback) {
        var dbInput = $("#db-input");
        dbInput.css("display", "block");
        dbInput.animate({paddingTop: '0px', opacity: 1}, 1000, function () {
          //console.log(callback);
          if (callback != null) callback();
        });
      }
    },
    mounted() {
      var self = this;
      self.showDBInput(function () {
        toastr.success("Welcome to DBComparator!");
      });

      /* Listen for events */
      bus.$on("showDBInput", function (data) {
        //console.log("[ EVENT ] - showDBInput");
        self.showDBInput();
      })

      window.onload = function () {

        /* Init the tooltip  */
        $("#db-name-one").tooltip();
        $("#db-name-two").tooltip();
        $("#servername-one").tooltip();
        $("#servername-two").tooltip();
        $("#db-username-one").tooltip();
        $("#db-username-two").tooltip();
        $("#db-password-one").tooltip();
        $("#db-password-two").tooltip();

        /* Init the toastr */
        toastr.options = {
          closeButton: true,
          progressBar: true,
          showMethod: 'slideDown',
          timeOut: 4000
        };

        /* Change the scroll style */
        $("#db-input").niceScroll({
          styler: "fb",
          cursorcolor: "rgb(201,201,201)",
          cursorwidth: '0',
          cursorborderradius: '0',
          autohidemode: 'true',
          background: '#1B2426',
          spacebarenabled: false,
          cursorborder: '0'
        });
      };
    }
  }
</script>

<style scoped>
  #db-input {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: white;
    z-index: 200;
    opacity: 0;
    padding-top: 30px;
    display: none;
    overflow: scroll;
  }

  .db-input-title {
    text-align: center;
    font-weight: 500;
    font-size: 60px;
  }

  .db-input-form {
    margin: 20px 0 30px;
  }

  .btn-compare-div {
    text-align: center;
  }

  .db-input-text {
    text-align: center;
  }
</style>
