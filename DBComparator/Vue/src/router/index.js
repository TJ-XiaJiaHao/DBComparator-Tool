import Vue from 'vue'
import Router from 'vue-router'
import Hello from '@/components/Hello'
import Header from '@/components/Header'
import MainPage from '@/components/MainPage'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/',
      name: 'Hello',
      components: {
        Header: Header,
        MainPage: MainPage
      }
    }, {
      path: '/mainpage',
      name: 'mainpage',
      components: {
        Header: Header,
        MainPage: MainPage
      }
    }
  ]
})
