<template>
  <div id="code-compare">
    <div class="half left">
      <h2 class="title">{{ dbname1 }} - {{ childname1 }}</h2>
      <div class="code-block code1">
        <pre class="brush: sql;">
          {{ code1 }}
        </pre>
      </div>
    </div>
    <div class="half right">
      <h2 class="title">{{ dbname2 }} - {{ childname2 }}</h2>
      <div class="code-block">
        <pre class="brush: sql;">
          {{ code2 }}
        </pre>
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
        code1:"select * from student where id = 1;",
        code2:"select * from student where id = 1;"
      }
    },
    methods:{
      showCode: function () {
          $("#code-compare").css("display","block");
      }
    },
    mounted() {
      SyntaxHighlighter.config.clipboardSwf = 'scripts/clipboard.swf';
      SyntaxHighlighter.all();

      var self = this;
      bus.$on("showCode",function(data){
        console.log("[ EVENT ] - showCode",data);
        self.dbname1 = data.dbname1;
        self.dbname2 = data.dbname2;
        self.childname1 = data.childname1;
        self.childname2 = data.childname2;
        $(".code1").html("<pre class='brush: sql;'>" + data.code1 +"</pre>")
        self.showCode();
      });
    }
  }
</script>

<style scoped>
  #code-compare {
    width: 100%;
    height: 100%;
    position: fixed;
    top:0;
    left:0;
    background: white;
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
    margin: 20px 0 10px;
  }

  /* code block */
  .code-block {
    width: 90%;
    height: calc(95% - 60px);
    margin: 2.5% 5%;
    background: #1B2426;
  }
</style>
