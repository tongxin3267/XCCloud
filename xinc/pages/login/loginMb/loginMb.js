// login.js



Page({

  /**  
   * 页面的初始数据
   */
  data: {
    Mbdata:{}
  },
  onLoad: function (options) {
    var that = this
    that.setData({
      Mbdata: JSON.parse(options.Mbdata),
      VerifyCode: "获取手机验证码"
    })
  },
  
  setVerify: function (e) {//发送验证码
    console.log(this.data.Mbdata)
    var linkTel = this.data.Mbdata;
    var _Url = "http://192.168.1.145:8080/xcgamemana/token?action=sendSMSCode";
    var total_micro_second = 60 * 1000;
    var that =this
    that.setData({
      disabled: !this.data.disabled
    })
    //验证码倒计时
    count_down(this, total_micro_second);
    wx.request({
      url: _Url,
      method: 'POST',
      headers: {
        'Content-Type': 'application/x-www-form-urlencoded'
      },
      data: {
        mobile: this.data.Mbdata.mobile,
        token: this.data.Mbdata.token
      },
      
      success: function (res) {
        console.log(res.data)
        if (res.data.result_code == 1) {
          wx.showModal({
            title: '提示',
            content: '发送验证码成功！',
            showCancel: false
          })
        }
      },
      fail: function (res) {
        console.log("error res=")
        console.log(res.data)
      }
    });
  },


  /**
   * 生命周期函数--监听页面加载
   */
  
 

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

//下面的代码在page({})外面
/* 毫秒级倒计时 */

function count_down(that, total_micro_second) {
  if (total_micro_second <= 0) {
    that.setData({
      VerifyCode: "重新发送",
      disabled: ""
    });
    // timeout则跳出递归
    return;
  }
  // 渲染倒计时时钟
  that.setData({
    VerifyCode: date_format(total_micro_second) + " 秒"
  });
  setTimeout(function () {
    // 放在最后--
    total_micro_second -= 10;
    count_down(that, total_micro_second);
  }, 10)
}
// 时间格式化输出，如03:25:19 86。每10ms都会调用一次
function date_format(micro_second) {
  // 秒数
  var second = Math.floor(micro_second / 1000);
  // 小时位
  var hr = Math.floor(second / 3600);
  // 分钟位
  var min = fill_zero_prefix(Math.floor((second - hr * 3600) / 60));
  // 秒位
  var sec = fill_zero_prefix((second - hr * 3600 - min * 60));// equal to => var sec = second % 60;
  // 毫秒位，保留2位
  var micro_sec = fill_zero_prefix(Math.floor((micro_second % 1000) / 10));
  return sec;
}
// 位数不足补零
function fill_zero_prefix(num) {
  return num < 10 ? "0" + num : num
}