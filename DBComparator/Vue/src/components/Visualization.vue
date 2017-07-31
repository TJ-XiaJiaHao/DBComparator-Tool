<template>
  <div id="visualization">
    <button type="button" class="btn btn-default btn-close" @click="close"> &times;</button>
    <div id="pie-chart"></div>
    <div id="details"></div>
  </div>
</template>

<script>
  import bus from "../assets/eventBus"
  export default {
    name: 'DBInput',
    data () {
      return {
        data: null,
        myChart: {
          currentType: "Tables",
          subText: "click a table to see the details",
          animationDuration: 1000,
          chart: null,
          option: null,
          data: {
            one: [],
            two: []
          },
        },
        details: {
          currentTable: "",
          isFirstIn:true,
          chart: null,
          option: null,
          data: {
            one: [],
            two: []
          }
        },
        widthDivision: 1,
        radiusDivision: 1
      }
    },
    methods: {
      /* Visualization IN or OUT animation */
      showVisualization: function () {
        $("#visualization").css("display", "block");
      },
      close: function () {
        $("#visualization").css("display", "none");
        this.widthDivision = 1;
        this.radiusDivision = 1;
        this.details.isFirstIn = true;
        this.clear();
      },

      clear: function () {
        this.myChart.chart.clear();
        this.details.chart.clear();
      },

      /* Pie Chart */
      highlightPieChart: function (index) {
        var begin = 0;
        var dataIndex = [];
        for (var i = 0; i < index; i++)
          begin += this.myChart.data.one[i].value;
        for (var i = begin; i < begin + this.myChart.data.one[index].value; i++)
          dataIndex.push(i);
        this.myChart.chart.dispatchAction({
          type: "highlight",
          seriesIndex: [1, 2, 3],
          dataIndex: dataIndex
        });
      },
      downplayPieChart: function () {
        this.myChart.chart.dispatchAction({
          type: "downplay"
        });
      },
      drawPieChart: function () {
        var self = this;
        this.resizeCharts();
        this.myChart.chart = echarts.init(document.getElementById('pie-chart'));
        this.myChart.option = this.getPieChartOption();
        this.myChart.chart.setOption(this.myChart.option);
        this.myChart.chart.off("click");
        this.myChart.chart.off("mouseover");
        this.myChart.chart.off("mouseout");
        this.myChart.chart.off("legendselectchanged");
        this.myChart.chart.on("click", function (paras) {
          if (paras.seriesIndex == 1) self.showDetails(paras.name);
        });
        this.myChart.chart.on("mouseover", function (paras) {
          if (paras.seriesIndex == 0) self.highlightPieChart(paras.dataIndex);
        })
        this.myChart.chart.on("mouseout", function (paras) {
          if (paras.seriesIndex == 0) self.downplayPieChart();
        })
        this.myChart.chart.on("legendselectchanged", function (paras) {
          switch (paras.name) {
            case "Tables":
              self.initChartTableOne();
              self.initChartTableTwo();
              self.myChart.subText = "click a table to see the details";
              break;
            case "Procedures":
              self.initChartProceOne();
              self.initChartProceTwo();
              self.myChart.subText = "";
              break;
            case "Functions":
              self.initChartFunctOne();
              self.initChartFunctTwo();
              self.myChart.subText = "";
              break;
            default:
              self.initChartTableOne();
              self.initChartTableTwo();
              self.myChart.subText = "click a table to see the details";
              break;
          }
          self.myChart.currentType = paras.name;
          self.myChart.option = self.getPieChartOption();
          self.myChart.chart.setOption(self.myChart.option);
        })
      },
      getPieChartOption: function () {
        var self = this;
        return {
          legend: {
            bottom: 5,
            data: ['Tables', 'Procedures', 'Functions'],
            itemGap: 20,
            textStyle: {
              color: '#fff',
              fontSize: 14
            },
            selectedMode: 'single',
            selected: {
//              'Procedures': false,
//              'Functions': false
            }
          },
          title: {
            text: "Different " + self.myChart.currentType,
            subtext: self.myChart.subText,
            left: "center",
            textStyle: {
              color: "#fff",
              fontSize: 18
            },
          },
          backgroundColor: new echarts.graphic.RadialGradient(0, 0, 1, [{
            offset: 0,
            color: '#174b78'
          },
            {
              offset: 1,
              color: '#1B2426'
            }]),
          tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b}:{c}({d}%)"
          },
          series: [{
            name: 'Count',
            type: 'pie',
            selectedMode: 'single',
            radius: [0, (40 / self.radiusDivision) + '%'],
            label: {
              normal: {
                position: 'inner'
              }
            },
            labelLine: {
              normal: {
                show: false
              }
            },
            data: self.myChart.data.one,
            animationDurationUpdate  : function (idx) {
              return self.myChart.animationDuration;
            }
          },
            {
              name: 'Tables',
              type: 'pie',
              radius: [(40 / self.radiusDivision + 2) + '%', (80 / self.radiusDivision) + '%'],
              label: {
                normal: {
                  position: 'outer'
                }
              },
              data: self.myChart.data.two,
              animationDurationUpdate  : function (idx) {
                return self.myChart.animationDuration;
              }
            },
            {
              name: 'Procedures',
              type: 'pie',
              radius: [(40 / self.radiusDivision + 2) + '%', (80 / self.radiusDivision) + '%'],
              label: {
                normal: {
                  position: 'outer'
                }
              },
              data: self.myChart.data.two,
              animationDurationUpdate  : function (idx) {
                return self.myChart.animationDuration;
              }
            },
            {
              name: 'Functions',
              type: 'pie',
              radius: [(40 / self.radiusDivision + 2) + '%', (80 / self.radiusDivision) + '%'],
              label: {
                normal: {
                  position: 'outer'
                }
              },
              data: self.myChart.data.two,
              animationDurationUpdate  : function (idx) {
                return self.myChart.animationDuration;
              }
            }]
        };
      },
      initChartTableOne: function () {
        this.myChart.data.one = [];
        var tables = this.data.tables;
        for (var i = 0; i < tables.length; i++) {
          var name = tables[i].coexist == true ? "coexist" : tables[i].dbnameIfNotCoexit;
          var flag = false;
          for (var j = 0; j < this.myChart.data.one.length; j++) {
            if (this.myChart.data.one[j].name == name) {
              this.myChart.data.one[j].value += 1;
              flag = true;
            }
          }
          if (!flag) {
            this.myChart.data.one.push({value: 1, name: name});
          }
        }
      },
      initChartTableTwo: function () {
        this.myChart.data.two = [];
        var tables = this.data.tables;
        var one = this.myChart.data.one;
        for (var i = 0; i < one.length; i++) {
          for (var j = 0; j < tables.length; j++) {
            if (one[i].name == "coexist" && tables[j].coexist) {
              this.myChart.data.two.push({value: 1, name: tables[j].name});
            }
            else if (one[i].name != "coexist" && !tables[j].coexist && tables[j].dbnameIfNotCoexit == one[i].name) {
              this.myChart.data.two.push({value: 1, name: tables[j].name});
            }
          }
        }
      },
      initChartProceOne: function () {
        this.myChart.data.one = [];
        var procedures = this.data.storedProcedures;
        for (var i = 0; i < procedures.length; i++) {
          var name;
          if (procedures[i].coexist) name = "coexist";
          else if (procedures[i].different[0].exist) name = procedures[i].different[0].dbname;
          else if (procedures[i].different[1].exist) name = procedures[i].different[1].dbname;
          var flag = false;
          for (var j = 0; j < this.myChart.data.one.length; j++) {
            if (this.myChart.data.one[j].name == name) {
              this.myChart.data.one[j].value += 1;
              flag = true;
            }
          }
          if (!flag) {
            this.myChart.data.one.push({value: 1, name: name});
          }
        }
      },
      initChartProceTwo: function () {
        this.myChart.data.two = [];
        var procedures = this.data.storedProcedures;
        var one = this.myChart.data.one;
        for (var i = 0; i < one.length; i++) {
          for (var j = 0; j < procedures.length; j++) {
            if (one[i].name == "coexist" && procedures[j].coexist) {
              this.myChart.data.two.push({value: 1, name: procedures[j].name});
            }
            else if (one[i].name != "coexist" && !procedures[j].coexist) {
              if (procedures[j].different[0].exist && procedures[j].different[0].dbname == one[i].name) {
                this.myChart.data.two.push({value: 1, name: procedures[j].name});
              }
              else if (procedures[j].different[1].exist && procedures[j].different[1].dbname == one[i].name) {
                this.myChart.data.two.push({value: 1, name: procedures[j].name});
              }
            }
          }
        }
      },
      initChartFunctOne: function () {
        this.myChart.data.one = [];
        var functions = this.data.functions;
        for (var i = 0; i < functions.length; i++) {
          var name;
          if (functions[i].coexist) name = "coexist";
          else if (functions[i].different[0].exist) name = functions[i].different[0].dbname;
          else if (functions[i].different[1].exist) name = functions[i].different[1].dbname;
          var flag = false;
          for (var j = 0; j < this.myChart.data.one.length; j++) {
            if (this.myChart.data.one[j].name == name) {
              this.myChart.data.one[j].value += 1;
              flag = true;
            }
          }
          if (!flag) {
            this.myChart.data.one.push({value: 1, name: name});
          }
        }
      },
      initChartFunctTwo: function () {
        this.myChart.data.two = [];
        var functions = this.data.functions;
        var one = this.myChart.data.one;
        for (var i = 0; i < one.length; i++) {
          for (var j = 0; j < functions.length; j++) {
            if (one[i].name == "coexist" && functions[j].coexist) {
              this.myChart.data.two.push({value: 1, name: functions[j].name});
            }
            else if (one[i].name != "coexist" && !functions[j].coexist) {
              if (functions[j].different[0].exist && functions[j].different[0].dbname == one[i].name) {
                this.myChart.data.two.push({value: 1, name: functions[j].name});
              }
              else if (functions[j].different[1].exist && functions[j].different[1].dbname == one[i].name) {
                this.myChart.data.two.push({value: 1, name: functions[j].name});
              }
            }
          }
        }
      },
      initPieChart: function () {
        this.initChartTableOne();
        this.initChartTableTwo();
        this.drawPieChart();
      },

      /* Details */
      drawDetails: function () {
        var self = this;
        this.resizeCharts();
        this.details.chart = echarts.init(document.getElementById('details'));
        this.details.option = this.getDetailsOption();
        this.details.chart.setOption(this.details.option);
      },
      getDetailsOption: function () {
        var self = this;
        return {
          title: {
            text: "Table details",
            subtext: self.details.currentTable,
            left: "center",
            textStyle: {
              color: "#fff",
              fontSize: 18
            },
          },

          backgroundColor: new echarts.graphic.RadialGradient(1, 0, 1, [{
            offset: 0,
            color: '#174b78'
          }, {
            offset: 1,
            color: '#1B2426'
          }]),
          tooltip: {
            trigger: 'item',
            formatter: "{a} <br/>{b}:{c}({d}%)"
          },
          series: [{
            name: 'Table',
            type: 'pie',
            selectedMode: 'single',
            radius: [0, (40 / self.radiusDivision) + '%'],
            label: {
              normal: {
                position: 'center',
                textStyle:{
                    color:"#fff"
                }
              }
            },
            labelLine: {
              normal: {
                show: false
              }
            },
            data: self.details.data.one
          }, {
            name: 'Details',
            type: 'pie',
            radius: [(40 / self.radiusDivision + 2) + '%', (80 / self.radiusDivision) + '%'],
            label: {
              normal: {
                position: 'inner'
              }
            },
            data: self.details.data.two
          }]
        };
      },
      initDetails: function (name) {
        if (name != null) this.details.currentTable = name;
        for (var i = 0; i < this.data.tables.length; i++) {
          var item = this.data.tables[i];
          if (this.data.tables[i].name == name) {
            this.details.data.one = [];
            this.details.data.two = [];
            this.details.data.one.push({value: 1, name: name});
            if (item.columns.length > 0)
              this.details.data.two.push({value: item.columns.length, name: "COLUMNS"});
            if (item.indexes.length > 0)
              this.details.data.two.push({value: item.indexes.length, name: "INDEXES"});
            if (item.keys.length > 0)
              this.details.data.two.push({value: item.keys.length, name: "KEYS"});
            if (this.details.data.two.length == 0)
              this.details.data.two.push({value: 0, name: "-"})
            break;
          }
        }
        this.drawDetails();
      },
      showDetails: function (name) {
        this.widthDivision = 2;
        this.radiusDivision = 1.5;
        this.resizeCharts();
        this.myChart.option = this.getPieChartOption();
        this.myChart.chart.setOption(this.myChart.option);
        if(this.details.isFirstIn){
          this.details.isFirstIn = false;
          setTimeout(this.initDetails,1000,name);
        }
        else{
          this.initDetails(name);
        }
      },

      /* Auto adapt the window */
      resizeCharts: function () {
        document.getElementById("pie-chart").style.width = window.innerWidth / this.widthDivision + "px";
        document.getElementById("pie-chart").style.height = window.innerHeight + 'px';
        document.getElementById("details").style.width = (window.innerWidth - window.innerWidth / this.widthDivision) + "px";
        document.getElementById("details").style.height = window.innerHeight + "px";
        if (this.myChart.chart != null) this.myChart.chart.resize();
        if (this.details.chart != null) this.details.chart.resize();
      }
    },
    mounted(){
      var self = this;

      /* Listen for events*/
      bus.$on("visualization", function (data) {
        self.data = data;
        self.showVisualization();
        self.initPieChart();
      })

      /* Auto adapt the window */
      window.onresize = function () {
        self.resizeCharts();
      }
    }
  }
</script>

<style scoped>
  #visualization {
    position: fixed;
    top: 0;
    left: 0;
    width: 100%;
    height: 100%;
    background: #1B2426;
    display: none;
    z-index: 1000;
  }

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
    z-index: 1005;
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

  #details, #pie-chart {
    float: left;
  }
</style>
