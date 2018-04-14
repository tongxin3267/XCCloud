
 /* 套餐
 */ 
function getIndexNavSectionData(){
    var arr = [
                [
                    {                        
                      subject:"50元50币",
                      coverpath:"../../../images/sales.png",
                      price:'¥198',
                      message:'50rmb现金支付'
                    },
                    {                       
                      subject:"50元50币",
                      coverpath:"../../../images/sales.png",
                      price:'¥188',
                      message:'50rmb现金支付'
                    },
                    {   
                      subject:"50元50币",
                      coverpath:"../../../images/sales.png",
                      price:'¥158',
                      message:'50rmb现金支付'
                    },
                    {   
                      subject:"50元50币",
                      coverpath:"../../../images/sales.png",
                      price:'¥19',
                      message:'50rmb现金支付'
                    },
                    {   
                      subject:"50元50币",
                      coverpath:"../../../images/sales.png",
                      price:'¥198',
                      message:'50rmb现金支付'
                    },
                    {
                      subject: "50元50币",
                      coverpath: "../../../images/sales.png",
                      price: '¥198',
                      message: '50rmb现金支付'
                    }
                ] 
            ]
    return arr
}


/*
 * 对外暴露接口
 */ 
module.exports = {
 
  getIndexNavSectionData : getIndexNavSectionData
 

}