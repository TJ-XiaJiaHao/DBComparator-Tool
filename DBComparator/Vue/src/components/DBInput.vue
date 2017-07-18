<template>
  <div id="db-input">
    <div class="col-md-3 col-sm-2"></div>

    <div class="col-md-6 col-sm-8 db-input-div">
      <h1 class="db-input-title">DATABASE ONE</h1>
      <form class="form-horizontal db-input-form" role="form">
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="servername-one" placeholder="server name"
                   title="you can get the server name in the sql server management tool!"
                   data-container="body" data-toggle="popover" data-placement="right" v-model="server1">
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="db-name-one" placeholder="database name"
                   title="please input the database name you want to compare"
                   data-container="body" data-toggle="popover" data-placement="right" v-model="dbname1">
          </div>
        </div>
      </form>
      <h1 class="db-input-title">DATABASE TWO</h1>
      <form class="form-horizontal db-input-form" role="form">
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="servername-two" placeholder="server name"
                   title="you can get the server name in the sql server management tool!"
                   data-container="body" data-toggle="popover" data-placement="right" v-model="server2">
          </div>
        </div>
        <div class="form-group">
          <div class="col-sm-12">
            <input type="text" class="form-control input-lg db-input-text" id="db-name-two" placeholder="database name"
                   title="please input the database name you want to compare"
                   data-container="body" data-toggle="popover" data-placement="right" v-model="dbname2">
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
        server1: "",
        dbname1: "",
        server2: "",
        dbname2: ""
      }
    },
    methods: {
      check: function () {
        if (this.server1 == "" || this.server2 == "" || this.dbname1 == "" || this.dbname2 == "") {
          toastr.error("null input are not be allowed!")
          return false;
        }
        return true;
      },
      submit: function () {
        console.log("[ SUBMIT ] - db1: " + this.server1 + " " + this.dbname1 + " ; db2: " + this.server2 + " " + this.dbname2);
        if (!this.check()) return;
        bus.$emit("compareDB", {
          server1: this.server1,
          dbname1: this.dbname1,
          server2: this.server2,
          dbname2: this.dbname2
        });
        this.hideDBInput();
      },
      hideDBInput: function () {
        var dbInput = $("#db-input");
        dbInput.animate({paddingTop: '30px', opacity: 0}, 1000, function () {
          dbInput.css("display", "none");
          bus.$emit("showHeader", "COMPARING...");
        })
      },
      showDBInput: function () {
        var dbInput = $("#db-input");
        dbInput.css("display", "block");
        dbInput.animate({paddingTop: '50px', opacity: 1}, 1000);
      }
    },
    mounted() {
      var self = this;

      bus.$on("showDBInput", function (data) {
        console.log("[ EVENT ] - showDBInput");
        self.showDBInput();
      })

      window.onload = function () {
        $("#servername-one").tooltip();
        $("#servername-two").tooltip();
        $("#db-name-one").tooltip();
        $("#db-name-two").tooltip();

        toastr.options = {
          closeButton: true,
          progressBar: true,
          showMethod: 'slideDown',
          timeOut: 4000
        };
        toastr.success("Welcome to DBComparator!");
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
    padding-top: 50px;
    background: white;
    z-index: 200;
  }

  .db-input-div {
    height: 800px;
  }

  .db-input-title {
    text-align: center;
    font-weight: 500;
    font-size: 60px;
  }

  .db-input-form {
    margin: 40px 0 50px;
  }

  .btn-compare-div {
    text-align: center;
  }

  .db-input-text {
    text-align: center;
  }
</style>
