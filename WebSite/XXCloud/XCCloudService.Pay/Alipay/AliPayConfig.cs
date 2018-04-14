using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XCCloudService.Pay.Alipay
{
    public class AliPayConfig
    {
        public static string alipay_public_key = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDI6d306Q8fIfCOaTXyiUeJHkrIvYISRcc73s3vF1ZT7XN8RNPwJxo8pWaJMmvyTn9N4HQ632qJBVHf8sxHi/fEsraprwCtzvzQETrNRwVxLO5jVmRGi60j8Ue1efIlzPXV9je9mkjzOmdssymZkh2QhUrCmZYI/FCEa3/cNMW0QIDAQAB";
        //这里要配置没有经过的原始私钥
        public static string merchant_private_key = "MIICXQIBAAKBgQDBkwdq6jV8DsW34UTQpeVa+zTDS1q5kkoOS/up+8xJvzTbDMj9RJpLdQPJyRdm8ynssaX/A+/I/emc7WjUdQ8k06MoZ7KZgxQ53alYnP782vZHzX8mbkmyO45JBpmB5mDqf6bUZ8eF0sruIxjBPrfZKY81Y2NGMb7GpUnRxglbAwIDAQABAoGAWlZ8mrfM2CWovkTiMaGKZShhGlc7culDqGJrg4vgbfZO+39++Tuf6mEksHIpesE2qqDJgDbdG+brtHHTf3tV4iDCCyCWMGasEsDYHrJdjDNMI3TRaPnFG7c0Gl3/3WfJt6+6kUWrA4yHpJ3671oW4vqYcZLClm4vvscAJI6iq5kCQQDsLqSQ+1HSTq6R/0Z6RWubq86tQaM8mHiVt/Q568E7vof4/WRAzm4RgHJFxSICsDNWHUOcXj51FMvDh3h4w5/lAkEA0dEjwL3iWKfNMbWs+GlxonULrbz7klU7ybaLjlPpqpD6u5vtwJIUeCsCz4t5F3ebGYy17ewvpcdTcJiLNvbQxwJBAKOmBf03RpB0WF1tBgZ+x3sL5p8CFftMonELDzx68F9XO5v451hIMNgLqiJR36kBMfD/QfO9EYXKrQ2EdqMm2UkCQQC5TZPd7dWxpsE6XdFdswKBEA44audj3ZLoGyg1kzaTRWeruZLIuDsLxjC9S9iuORvp/LPQZP/P0bKHhrGs4V6RAkBcd6P8QwkeMpHG471oa9dxU9SXn8YC4nK7ePEJ/F9g023L2t8DO2heBuD/SyQrtYu1TXSuMqc3t/ZRFspUM8MF";
        //public static string merchant_public_key = @"MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDBkwdq6jV8DsW34UTQpeVa+zTDS1q5kkoOS/up+8xJvzTbDMj9RJpLdQPJyRdm8ynssaX/A+/I/emc7WjUdQ8k06MoZ7KZgxQ53alYnP782vZHzX8mbkmyO45JBpmB5mDqf6bUZ8eF0sruIxjBPrfZKY81Y2NGMb7GpUnRxglbAwIDAQAB";
        public static string appId = "2016051601406637";    //当面付APPID
        public static string serverUrl = "https://openapi.alipay.com/gateway.do";
        public static string mapiUrl = "https://mapi.alipay.com/gateway.do";
        public static string monitorUrl = "http://mcloudmonitor.com/gateway.do";
        public static string pid = "2088221884894147";


        public static string charset = "utf-8";//"utf-8";
        public static string sign_type = "RSA2";
        public static string version = "1.0";

        public static string notify_url = ConfigurationManager.AppSettings["AlipayNotifyUrl"];

        /// <summary>
        /// 莘拍档小程序支付宝公钥
        /// </summary>
        public static string alipay_miniapp_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAgZ1Ry5lacgbU48fsrhQsGtJIF6mgL7c+Rz+WNdTrMovgNydsLUTL6w0e+UpJI9JrAN7WHCVlOUChXQmQ2PkSfcjhwMlZFgE7Zf9G4Y+aI917daWqEpLpJHcpMNr9jE10ctsjxKAHl/hVHMRYDMc4JlOSisOOspTeCRePSq6LKT4zFaQWgW6/Gs/r8Og6+QGYpph3dmMogNqGfm502DWeUDRoJ/tZyPFCA0h9kJP0ZVlnN1T1Miezbti6pBOsyw8cFQYmIyFeYQpWz4hSApHCN454SFSRwVT3/dgS/s81Hmb8hkIaZ/uZ+EKPiFFpg3HJs7dyjfHdlPhlYL38iboKqQIDAQAB";
        /// <summary>
        /// 莘拍档小程序商户私钥
        /// </summary>
        public static string merchant_miniapp_private_key = "MIIEpAIBAAKCAQEAwmjIXqI40SpGlcHhpoJOxptSJoB6SPvsfaSNTXWOdBlH5ClGh8iMp6dZ0yGcu+0nhhBajLPJRKMb0G9isZnfm2rT+rfxPFYuoImgcAAMSW8HFBHx/B7ObX14bwX9l1PXZOF0cXttrEnHBCgfCZPldm4vCUS4jY3vyuuLiZQnX+iFysS+vwqrYPlVh4mn1dgI32QtOe5bsX3DPK/6x3cX1HwLFXohZ6dav8EjWmvw50zRQZeXjy7bQzebdj19zdZiuZmHP1kXvdUPmZcDyXZRreLDCQa38VNWuiggOU5ZGgd4UGG7VBvA4oY7BWQvLYFhRfJDq3EETMYOUNoMU7efjwIDAQABAoIBAEuqAooqcxidbplsu0lWIBjWbNPFX1K2kiWYfNJv9YCkMkHP+r5wRKYf0SsGQv01YROhm8x1UNlg7jtijrP24nuXyWckXkSQeldLGWFd82pFRyj9jwQEbW1ZzxzO5RMXs5Z01SAx3M6YbFfm0aWlpUJBwhbGY1ncqz58DHQAcI63v13fG505H7IkUVobZxm6SLaleqxAOR2OLiM89noQ+3ZzEMZWj2uI1ODVMubDfMNRiiFfRxvlipd04+BrJRGS3s1170135G2+VpX3w8f0ffRkRMcuVpBNpHI/NlImeHI+p/aGBtp887q8WD0bY2f7M1zdDudMd4Rn7jP3OVakfDkCgYEA+Qeno0mg7VRdw1z5QwCED7ZpfraEdgfl3pfW1VthsQcrbo8KwQLPH85ZNIo9iR/2xVzmd1egBd1NGl8hKQRODtSEi2tQd2eGzFy3NiGWl5nawgnTsh9pb7TEDVKV3ppahfE3dEzL+ATgMXu+/bdy4g+SBssX5IHmFMmy9s4Hb5UCgYEAx9nC3+YimtGQJP7T3e6TbezJ5CkybANEIcWWDihK8NbS+5iytBtqVj7bSNnFsJCk9/tcM9i6GIrD3Exsk+3vdWc8x9hq7Kh03AzEFcz3SIl0owfwnTGHasp10pgRJQf1qztDy+BDAYt6tcn2cXhRm3bi1aXU9LUNF7BEjE4ZGZMCgYEAhEDZebGfoor8pMVap4e9mu6gRig+XsTBAp8W/AZk8nRBT5zt9CU+xMB8xZdpKMrt7lDxII5LsPGfghLldHgR0HRy0+wxNsinYR8YOcpKxMZz1DsNz3o5L5cLy5uBdk/5JUs+zSf/5v8H5Z+3hd0ISSBFyA/R54xJGd8oiURP1KUCgYEAke/Jp1hVcrqPSQSw0AJ0tDPSZgNTZ56m9QZlAieYOfCVltY1wOG4MAyscrFb9Ahb/x4VgWLE2kESr187dnasgw4FS2YVBoYo97t2wPiwps6BVr/oi9FsMqZvjX4wHtuGISkg49L6+zjQPcmiZc/Xv+/7ysnTDXLcgazdEZvCJnMCgYBoxzvVxcPuE84tnHTYsHGE5bVns9lYHkJqPfRZ6Z5TR8nm03StgxeS5WVnm3pTi8Bm725AQ5I2+2N8N1yc5VF627nsEquYsyJutj+NgH9rP3UwXvf+01SUXtx2umlxqIL1QV7AQ6Qs8o8UXUnDg9Cp6xY/0pw24IeEXtxD4whedw==";
        /// <summary>
        /// 莘拍档APPID
        /// </summary>
        public static string miniAppId = "2018032302432281";

        /// <summary>
        /// 支付宝莘拍档小程序模版消息支付成功模版ID
        /// </summary>
        public static string MiniAppTemplateId = "NmFkODNlNGQwNzlhNzVhMTBkYzY1ODJkZTI0YmRkMzA=";

        /// <summary>
        /// 莘拍档支付宝小程序支付回调
        /// </summary>
        public static string AliMiniAppNotify_url = ConfigurationManager.AppSettings["XcGameManageAliMiniAppNotifyUrl"];

        /// <summary>
        /// 支付宝授权后跳转到的前端地址
        /// </summary>
        public static string AliAuthRedirectUrl = ConfigurationManager.AppSettings["AliAuthRedirectUrl"];


        /// <summary>
        /// 支付宝商户用公钥（应用：莘宸娃娃机乐园，用于支付宝授权）
        /// </summary>
        public static string alipay_auth_public_key = "MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAgZ1Ry5lacgbU48fsrhQsGtJIF6mgL7c+Rz+WNdTrMovgNydsLUTL6w0e+UpJI9JrAN7WHCVlOUChXQmQ2PkSfcjhwMlZFgE7Zf9G4Y+aI917daWqEpLpJHcpMNr9jE10ctsjxKAHl/hVHMRYDMc4JlOSisOOspTeCRePSq6LKT4zFaQWgW6/Gs/r8Og6+QGYpph3dmMogNqGfm502DWeUDRoJ/tZyPFCA0h9kJP0ZVlnN1T1Miezbti6pBOsyw8cFQYmIyFeYQpWz4hSApHCN454SFSRwVT3/dgS/s81Hmb8hkIaZ/uZ+EKPiFFpg3HJs7dyjfHdlPhlYL38iboKqQIDAQAB";
        /// <summary>
        /// 支付宝商户私钥（应用：莘宸娃娃机乐园，用于支付宝授权）
        /// </summary>
        public static string merchant_auth_private_key = "MIIEpAIBAAKCAQEAwmjIXqI40SpGlcHhpoJOxptSJoB6SPvsfaSNTXWOdBlH5ClGh8iMp6dZ0yGcu+0nhhBajLPJRKMb0G9isZnfm2rT+rfxPFYuoImgcAAMSW8HFBHx/B7ObX14bwX9l1PXZOF0cXttrEnHBCgfCZPldm4vCUS4jY3vyuuLiZQnX+iFysS+vwqrYPlVh4mn1dgI32QtOe5bsX3DPK/6x3cX1HwLFXohZ6dav8EjWmvw50zRQZeXjy7bQzebdj19zdZiuZmHP1kXvdUPmZcDyXZRreLDCQa38VNWuiggOU5ZGgd4UGG7VBvA4oY7BWQvLYFhRfJDq3EETMYOUNoMU7efjwIDAQABAoIBAEuqAooqcxidbplsu0lWIBjWbNPFX1K2kiWYfNJv9YCkMkHP+r5wRKYf0SsGQv01YROhm8x1UNlg7jtijrP24nuXyWckXkSQeldLGWFd82pFRyj9jwQEbW1ZzxzO5RMXs5Z01SAx3M6YbFfm0aWlpUJBwhbGY1ncqz58DHQAcI63v13fG505H7IkUVobZxm6SLaleqxAOR2OLiM89noQ+3ZzEMZWj2uI1ODVMubDfMNRiiFfRxvlipd04+BrJRGS3s1170135G2+VpX3w8f0ffRkRMcuVpBNpHI/NlImeHI+p/aGBtp887q8WD0bY2f7M1zdDudMd4Rn7jP3OVakfDkCgYEA+Qeno0mg7VRdw1z5QwCED7ZpfraEdgfl3pfW1VthsQcrbo8KwQLPH85ZNIo9iR/2xVzmd1egBd1NGl8hKQRODtSEi2tQd2eGzFy3NiGWl5nawgnTsh9pb7TEDVKV3ppahfE3dEzL+ATgMXu+/bdy4g+SBssX5IHmFMmy9s4Hb5UCgYEAx9nC3+YimtGQJP7T3e6TbezJ5CkybANEIcWWDihK8NbS+5iytBtqVj7bSNnFsJCk9/tcM9i6GIrD3Exsk+3vdWc8x9hq7Kh03AzEFcz3SIl0owfwnTGHasp10pgRJQf1qztDy+BDAYt6tcn2cXhRm3bi1aXU9LUNF7BEjE4ZGZMCgYEAhEDZebGfoor8pMVap4e9mu6gRig+XsTBAp8W/AZk8nRBT5zt9CU+xMB8xZdpKMrt7lDxII5LsPGfghLldHgR0HRy0+wxNsinYR8YOcpKxMZz1DsNz3o5L5cLy5uBdk/5JUs+zSf/5v8H5Z+3hd0ISSBFyA/R54xJGd8oiURP1KUCgYEAke/Jp1hVcrqPSQSw0AJ0tDPSZgNTZ56m9QZlAieYOfCVltY1wOG4MAyscrFb9Ahb/x4VgWLE2kESr187dnasgw4FS2YVBoYo97t2wPiwps6BVr/oi9FsMqZvjX4wHtuGISkg49L6+zjQPcmiZc/Xv+/7ysnTDXLcgazdEZvCJnMCgYBoxzvVxcPuE84tnHTYsHGE5bVns9lYHkJqPfRZ6Z5TR8nm03StgxeS5WVnm3pTi8Bm725AQ5I2+2N8N1yc5VF627nsEquYsyJutj+NgH9rP3UwXvf+01SUXtx2umlxqIL1QV7AQ6Qs8o8UXUnDg9Cp6xY/0pw24IeEXtxD4whedw==";
        /// <summary>
        /// APPID（应用：莘宸娃娃机乐园，用于支付宝授权）
        /// </summary>
        public static string authAppId = "2018032302435860";
    }

    public class XCPayConfig
    {
        //这里要配置没有经过的原始私钥x
        public static string XCAlipayMPRK = "MIICXQIBAAKBgQDBkwdq6jV8DsW34UTQpeVa+zTDS1q5kkoOS/up+8xJvzTbDMj9RJpLdQPJyRdm8ynssaX/A+/I/emc7WjUdQ8k06MoZ7KZgxQ53alYnP782vZHzX8mbkmyO45JBpmB5mDqf6bUZ8eF0sruIxjBPrfZKY81Y2NGMb7GpUnRxglbAwIDAQABAoGAWlZ8mrfM2CWovkTiMaGKZShhGlc7culDqGJrg4vgbfZO+39++Tuf6mEksHIpesE2qqDJgDbdG+brtHHTf3tV4iDCCyCWMGasEsDYHrJdjDNMI3TRaPnFG7c0Gl3/3WfJt6+6kUWrA4yHpJ3671oW4vqYcZLClm4vvscAJI6iq5kCQQDsLqSQ+1HSTq6R/0Z6RWubq86tQaM8mHiVt/Q568E7vof4/WRAzm4RgHJFxSICsDNWHUOcXj51FMvDh3h4w5/lAkEA0dEjwL3iWKfNMbWs+GlxonULrbz7klU7ybaLjlPpqpD6u5vtwJIUeCsCz4t5F3ebGYy17ewvpcdTcJiLNvbQxwJBAKOmBf03RpB0WF1tBgZ+x3sL5p8CFftMonELDzx68F9XO5v451hIMNgLqiJR36kBMfD/QfO9EYXKrQ2EdqMm2UkCQQC5TZPd7dWxpsE6XdFdswKBEA44audj3ZLoGyg1kzaTRWeruZLIuDsLxjC9S9iuORvp/LPQZP/P0bKHhrGs4V6RAkBcd6P8QwkeMpHG471oa9dxU9SXn8YC4nK7ePEJ/F9g023L2t8DO2heBuD/SyQrtYu1TXSuMqc3t/ZRFspUM8MF";
        public static string XCAlipayMPUK = "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQDDI6d306Q8fIfCOaTXyiUeJHkrIvYISRcc73s3vF1ZT7XN8RNPwJxo8pWaJMmvyTn9N4HQ632qJBVHf8sxHi/fEsraprwCtzvzQETrNRwVxLO5jVmRGi60j8Ue1efIlzPXV9je9mkjzOmdssymZkh2QhUrCmZYI/FCEa3/cNMW0QIDAQAB";
        public static string XCAliPayAppId = "2016051601406637";    //当面付APPID
        public static string serverUrl = "https://openapi.alipay.com/gateway.do";
        public static string mapiUrl = "https://mapi.alipay.com/gateway.do";
        public static string monitorUrl = "http://mcloudmonitor.com/gateway.do";
        public static string XCAliPayPID = "2088221884894147";

        //public static string XCWxAppID = "wx86275e2035a8089d";
        //public static string XCWxMchID = "1480638152";
        //public static string XCWxKey = "Whxckj51530888888888888888888888";
        public static string XCWxAppID = "wxab2c44b2c5e18889";
        public static string XCWxMchID = "1391813202";
        public static string XCWxKey = "Whxckj40000515308888888888888888";
        public static string IP = "123.56.111.145";
    }
}
