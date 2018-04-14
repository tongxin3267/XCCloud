var app = getApp()
Page({
  data: {
    show: "",
  },

  onLoad: function () {
    
  },
  click: function () {
    var that = this;
    var show;
    
    wx.scanCode({
      success: (res) => {
        this.show = "--result:" + res.result;
        var str = this.show
        
        str = str.split("/");
        str = str[str.length-1]
        this.show = str
        //  this.show = "--result:" + res.result + "--scanType:" + res.scanType + "--charSet:" + res.charSet + "--path:" + res.path;
        that.setData({
          show: this.show
        })
        wx.request({
          url: "http://op.juhe.cn/onebox/weather/query",
          header: {
            "Content-Type": "application/x-www-form-urlencoded"
          },
          method: "POST",

          data: ({ cityname: "上海", key: "1430ec127e097e1113259c5e1be1ba70" }),
          complete: function (res) {
            var that = this
            that.setData({
              toastHidden: false,
              toastText: res.data.reason,
              city_name: res.data.result.data.realtime.city_name,
              date: res.data.result.data.realtime.date,
              info: res.data.result.data.realtime.weather.info,
            });
            if (res == null || res.data == null) {
              console.error('网络请求失败');
              return;
            }
          }
        })
        

        wx.showToast({
        
          title: '成功',
          icon: 'success',
          duration: 2000
        })
      },
      fail: (res) => {
        wx.showToast({
          title: '失败',
          icon: 'success',
          duration: 2000
        })
      },
      complete: (res) => {
      }
    }) 
  }
})