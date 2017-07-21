<template>
  <div id="app">
    <router-view name="Header"></router-view>
    <router-view name="MainPage"></router-view>
  </div>
</template>

<script>
  export default {
    name: 'app',
    mounted() {
      /* Change the scroll style */
      $(document).ready(function () {
        $("#app").niceScroll({
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
    }
  }
</script>

<style>
  #app {
    font-family: 'Avenir', Helvetica, Arial, sans-serif;
    -webkit-font-smoothing: antialiased;
    -moz-osx-font-smoothing: grayscale;
    color: #2c3e50;
    width: 100%;
    height: 100%;
    overflow: scroll;
  }

  * {
    margin: 0;
    padding: 0;
  }

  html, body {
    width: 100%;
    height: 100%;
  }
</style>
