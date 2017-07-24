<template>
  <div id="code-compare">
    <button type="button" class="btn btn-default btn-close" @click="close"> &times;</button>
    <div class="half left">
      <h2 class="title" v-bind:title="dbname1 + ' - ' + childname1">{{ dbname1 }} - {{ childname1 }}</h2>
      <div class="code-block code1">
      </div>
    </div>
    <div class="half right">
      <h2 class="title" v-bind:title="dbname2 + ' - ' + childname2">{{ dbname2 }} - {{ childname2 }}</h2>
      <div class="code-block code2">
      </div>
    </div>
  </div>
</template>

<script>
  import bus from "../assets/eventBus"
  export default {
    name: 'CodeCompare',
    data () {
      return {
        dbname1: "dbname1",
        dbname2: "dbname2",
        childname1: "fname1",
        childname2: "fbane2",
      }
    },
    methods: {

      /* Show the page and highlight the codes */
      showCode: function () {
        $("#code-compare").css("display", "block").animate({height: '100%'}, 500, function () {
          $(".code-block").animate({marginTop: '0', opacity: '1'}, 1000);
          $(".left .title").animate({marginLeft: '5%', opacity: '1'}, 1000);
          $(".right .title").animate({marginLeft: '5%', opacity: '1'}, 1000);
        });
        SyntaxHighlighter.highlight();
      },

      /* Close the page, set the display to none */
      close: function () {
        $(".left .title").animate({marginLeft: '0px', opacity: '0'}, 500);
        $(".right .title").animate({marginLeft: '0px', opacity: '0'}, 500);
        $(".code-block").animate({marginTop: '30px', opacity: '0'}, 500);
        $("#code-compare").animate({height: '0'}, 500, function () {
          $(this).css("display", "none");
          $(".code-block").css("marginTop","30px").css("opacity","0");
        });
      }
    },
    mounted() {
      /* Render the Html to highlight the code when window loaded */
      SyntaxHighlighter.config.clipboardSwf = 'scripts/clipboard.swf';
      SyntaxHighlighter.all();

      var self = this;

      /* Listen for events */
      bus.$on("showCode", function (data) {
        console.log("[ EVENT ] - showCode", data);
        self.dbname1 = data.dbname1;
        self.dbname2 = data.dbname2;
        self.childname1 = data.childname1;
        self.childname2 = data.childname2;
        $(".code1").html("<pre class='brush: sql;'>" + data.code1 + "</pre>");
        $(".code2").html("<pre class='brush: sql;'>" + data.code2 + "</pre>");
        self.showCode();
      });

      /* Change the scroll style */
      $(document).ready(function () {
        $(".code-block").niceScroll({
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
    },
  }
</script>

<style scoped>

  /* Close button */
  .btn-close {
    width: 100px;
    height: 100px;
    -webkit-border-radius: 50%;
    -moz-border-radius: 50%;
    border-radius: 50%;
    position: absolute;
    right: -70px;
    top: -70px;
    color: white;
    border: none;
    background: gray;
    font-size: 70px;
    opacity: 0.5;
    -webkit-transition: all 0.5s;
    -moz-transition: all 0.5s;
    -ms-transition: all 0.5s;
    -o-transition: all 0.5s;
    transition: all 0.5s;
    /*right: 2.5%;*/
    /*top: 10px;*/
  }

  .btn-close:hover {
    right: -50px;
    top: -50px;
    opacity: 1;
    padding: 20px 30px 20px 0;
  }

  .btn-close:focus,
  .btn-close:active:focus,
  .btn-close.active:focus,
  .btn-close.focus,
  .btn-close:active.focus,
  .btn-close.active.focus {
    outline: none;
  }

  /* Main page */
  #code-compare {
    width: 100%;
    height: 0;
    position: fixed;
    top: 0;
    left: 0;
    background: #1B2426;
    z-index: 1000;
    display: none;
  }

  .half {
    width: 50%;
    height: 100%;
    float: left;
  }

  .left {
    border-right: 1px dotted blue;
  }

  /* Title,about database name and the function or procedure name */
  .title {
    text-align: center;
    height: 30px;
    margin: 20px 0 10px 5%;
    color: #5BA1CF;
    opacity:0;
    font-size:28px;
    line-height: 30px;
    overflow: hidden;
    max-width:90%;
    white-space:nowrap;
  }
  .left .title{
    margin-left:-30px;
  }
  .right .title{
    margin-left:30px;
  }

  /* code block */
  .code-block {
    width: 90%;
    height: calc(95% - 60px);
    margin: 2.5% 5%;
    background: #1B2426;
    overflow-Y: scroll;
    margin-top:30px;
    opacity:0;
  }
</style>
