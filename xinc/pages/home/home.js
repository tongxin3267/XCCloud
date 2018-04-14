// home.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    banners: [
      { 'picUrl': 'http://img4.imgtn.bdimg.com/it/u=2192668381,2116711447&fm=200&gp=0.jpg'},
      { 'picUrl': 'http://imgstore.cdn.sogou.com/app/a/100540002/760050.jpg'},
      { 'picUrl': 'http://img2.niutuku.com/desk/anime/2520/2520-8688.jpg'},
    ],
    techNews: []
    
  },
  
 
  toDetail:function(event){
      var book = event.currentTarget.dataset.book;
      wx.navigateTo({
        url: '/pages/bookDetail/bookDetail?book='+JSON.stringify(book),
      })
  },
  toHotPush: function(event){
    var index=event.currentTarget.dataset.index;
    if (index==0){
      wx.navigateTo({
        url: '/pages/hotPush/hotPush',
      })
    }else{
      wx.showToast({
        title: '暂无此功能!',
        duration: 1000,

      })
    }
    
  },
  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onLoad: function () {
    var that = this
    // 访问聚合数据的网络接口-头条新闻
    wx.request({
      url: 'http://v.juhe.cn/toutiao/index',
      data: {
        type: 'topNews',
        key: '482e213ca7520ff1a8ccbb262c90320a'
      },
      header: {
        'Content-Type': 'application/json'
      },
      success: function (res) {
        if (res.data.error_code == 0) {
          that.setData({
            topNews: res.data.result.data
          })
        } else {
          console.log('获取失败');
        }
      }
    })
    // 访问聚合数据的网络接口-科技新闻
    wx.request({
      url: 'http://v.juhe.cn/toutiao/index',
      data: {
        type: 'keji',
        key: '482e213ca7520ff1a8ccbb262c90320a'
      },
      header: {
        'Content-Type': 'application/json'
      },
      success: function (res) {
        if (res.data.error_code == 0) {
          that.setData({
            techNews: res.data.result.data
          })
        } else {
          console.log('获取失败');
        }
      }
    })

  },

  /**
   * 生命周期函数--监听页面显示
   */
  onShow: function () {

  },

  /**
   * 生命周期函数--监听页面隐藏
   */
  onHide: function () {

  },

  /**
   * 生命周期函数--监听页面卸载
   */
  onUnload: function () {

  },

  /**
   * 页面相关事件处理函数--监听用户下拉动作
   */
  onPullDownRefresh: function () {

  },

  /**
   * 页面上拉触底事件的处理函数
   */
  onReachBottom: function () {

  },

  /**
   * 用户点击右上角分享
   */
  onShareAppMessage: function () {

  }
})