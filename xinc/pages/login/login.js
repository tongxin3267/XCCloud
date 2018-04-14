// login.js
Page({

  /**  
   * 页面的初始数据
   */
  data: {
    account: '',
    password: ''
  },

  accountInput: function (event) {

    this.setData({
      account: event.detail.value
    });
  },
  passwordInput: function (event) {

    this.setData({
      password: event.detail.value
    });
  },
  login: function (event) {
    var that =this
    
    // console.log(this.data);
    wx.request({
      url: 'http://192.168.1.145:8080/xcgamemana/token?action=checkImgCode',
      method: 'POST',
      header: {
         'content-type': 'application/x-www-form-urlencoded'
          
      },
      data: { 
        sysId  : 0,
        mobile : this.data.account,
        code: this.data.password
      },
      success: function (res) {
        // console.log(res.data);
        
        
        if (res.data.Result_Code == 1){
         
        wx.navigateTo({
          url: 'loginMb/loginMb?Mbdata=' + JSON.stringify(res.data.Result_Data),

        })
        } else{
          that.setData({
            success: "验证码错误，请重新输入",
            Verification: "http://192.168.1.145:8080/ServicePage/ValidateImg.aspx? t =" + new Date().getTime()

          })

        }
       
      },
      fail: function (err) {
         
        that.setData({
          success: "请求失败，请一会再试"
        })
        // console.log(err.data);
      }
    });
  },
  /**
   * 生命周期函数--监听页面加载
   */


  Invisibility:function(){
    
    
    var that =this
    
      // console.log(new Date().getTime())
      that.setData({
        Verification: "http://192.168.1.145:8080/ServicePage/ValidateImg.aspx? t =" + new Date().getTime()

      })
      
    
  },
    

  onLoad: function (options) {
   
    var that =this
    that.setData({
      Verification: "http://192.168.1.145:8080/ServicePage/ValidateImg.aspx? t =" + new Date().getTime()
    })
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