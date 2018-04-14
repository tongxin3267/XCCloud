//index.js
//获取应用实例
var app = getApp()
var fileData = require('../mall/data.js')

Page({
  // 页面初始数据
  data: { 
    // nav 初始化
    
    navSectionItems: fileData.getIndexNavSectionData(),
    
    curIndex: 0
  },

  onLoad: function () {
    var that = this
    that.setData({
      list: that.data.navSectionItems
    })
  },
 

  

})
