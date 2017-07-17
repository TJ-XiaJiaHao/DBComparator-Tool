<template>
  <div id="main" class="ag-bootstrap">
  </div>
</template>

<script>
  export default {
    name: 'DBInput',
    data () {
      return {
        msg: 'Welcome to Your Vue.js App'
      }
    }
  }
</script>

<style scoped>
  #main{
    width:100%;
    height:calc(100% - 50px);
    padding:20px 50px 10px 100px;
    opacity:0;
    text-align: center;
  }
  .ag-header-cell-menu-button{
    position: absolute !important;
    right:0;
  }
  .ag-body-container{
    float:left;
  }
</style>
