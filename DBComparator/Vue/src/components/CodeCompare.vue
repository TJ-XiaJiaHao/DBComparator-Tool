<template>
  <div id="code-compare">
    <button type="button" class="btn btn-default btn-close" @click="close">CLOSE</button>
    <div class="half left">
      <h2 class="title">{{ dbname1 }} - {{ childname1 }}</h2>
      <div class="code-block code1">
      </div>
    </div>
    <div class="half right">
      <h2 class="title">{{ dbname2 }} - {{ childname2 }}</h2>
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
      showCode: function () {
        $("#code-compare").css("display", "block").animate({height:'100%'},500);
        SyntaxHighlighter.highlight();
      },
      close:function(){
        $("#code-compare").animate({height:'0'},500,function(){
            $(this).css("display", "none");
        });
      }
    },
    mounted() {
      SyntaxHighlighter.config.clipboardSwf = 'scripts/clipboard.swf';
      SyntaxHighlighter.all();

      var self = this;
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

      $(document).ready(function(){
        $("html").niceScroll({styler:"fb",cursorcolor:"rgb(16, 167, 175)", cursorwidth: '0', cursorborderradius: '10px', background: '#424f63', spacebarenabled:false, cursorborder: '0',  zindex: '1000'});
        $(".code-block").niceScroll({styler:"fb",cursorcolor:"rgb(201,201,201)", cursorwidth: '0', cursorborderradius: '0',autohidemode: 'true', background: '#1B2426', spacebarenabled:false, cursorborder: '0'});
      });
    },
    updated() {
    }
  }
</script>

<style scoped>
  #code-compare {
    width: 100%;
    height: 0;
    position: fixed;
    top: 0;
    left: 0;
    background: white;
    z-index: 1000;
    display: none;
  }

  .btn-close{
    position: absolute;
    right:2.5%;
    top:10px;
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
    margin: 20px 0 10px;
  }

  /* code block */
  .code-block {
    width: 90%;
    height: calc(95% - 60px);
    margin: 2.5% 5%;
    background: #1B2426;
    overflow-Y: scroll;
  }
</style>
