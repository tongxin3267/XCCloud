// bookList.js
Page({

  /**
   * 页面的初始数据
   */
  data: {
    bookList: [],
  },
  saveM: function(){
    var that =this
    that.setData({
      hidden: "show",
      hidden1: "hidden",
      show: "hidden",
    })
    setTimeout(function () {
      that.setData({
        hidden: "hidden",
        show: "show",
        hidden1: "show",
        memberText1: "******1号",
        memberText2: "可用",
        memberText3: "*****",
        memberText4: "莘拍档畅玩通卡",
        memberText5: "564549684313842531",
        memberText6: "19999",
        memberText7: "600",
        memberText8: "继续存币",
        memberText9: "show",
        number: "10000"
      });
      that.update();
    }, 3000);
  },
  /**
   * 生命周期函数--监听页面加载
   */
  onLoad: function (options) {
    this.setData({
      show: "hidden",
      hidden: "hidden",
      memberText1: "******1号",
      memberText2: "可用",
      memberText3: "*****",
      memberText4: "莘拍档畅玩通卡",
      memberText5: "564549684313842531",
      memberText6: "9999",
      memberText7: "5000",
      memberText8: "开始存币",
      memberText9: "show",
    });
    
  },
  
  
  
  /**
   * 生命周期函数--监听页面初次渲染完成
   */
  onReady: function () {

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