var pwd_status = 0;
//用戶資安驗證
//參數1：機敏頁面代碼，2：機敏資料主鍵，3：是否彈出驗證密碼框,4：是否直接顯示機敏信息5.編輯的方法名稱6.驗證通過后是否打開窗口5
// 7:客戶信息類型user:會員 order：訂單 vendor：供應商 8：客戶id9：要顯示的客戶信息
function SecretLoginFun(type, relatedID, isLogin, isShow, isEdit, urlType, info_type, info_id, secret_info) {
    var FrmSec = Ext.create('Ext.form.Panel', {
        id: 'FrmSec',
        frame: true,
        autoScroll: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 45,
        url: '/SecretInfo/SecretLogin',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '管理員設定密碼',
                id: 'oldpassword',
                name: 'oldpassword',
                inputType: 'password',
                allowBlank: pwd_status == 0 ? false : true,
                hidden: pwd_status == 0 ? false : true
            },

           {
               xtype: 'textfield',
               fieldLabel: pwd_status == 0 ? '新 設 定 密 碼 ' : '資安密碼',
               id: 'password',
               name: 'password',
               inputType: 'password',
               minLength: 8,
               allowBlank: false
           },
                 {
                     xtype: 'fieldcontainer',
                     combineErrors: false,
                     layout: 'hbox',
                     items: [
                         {
                             xtype: 'textfield',
                             fieldLabel: '驗證碼',
                             id: 'check_code',
                             name: 'check_code',
                             margin: '0 5 0 0',
                             allowBlank: false,
                             width: 254,
                             submitValue: true,
                             listeners: {
                                 blur: function () {
                                     var oVal = Ext.getCmp('check_code');
                                     if (oVal.getValue() != "") {
                                         var reg = /\s/g;
                                         if (document.getElementById("key").innerHTML.replace(reg, '') != oVal.getValue()) {
                                             Ext.Msg.alert(INFORMATION, "驗證碼錯誤！");
                                             oVal.setValue("");
                                             Ext.getCmp('showcode').fireEvent('click');
                                         }
                                     }
                                 }
                                 ,
                                 specialkey: function (field, e) {
                                     if (e.getKey() == e.ENTER) {
                                         Ext.getCmp('btnSub').fireEvent('click');
                                     }
                                 }
                             }
                         },
                        {
                            xtype: 'button',
                            cls: 'icon-code',
                            name: 'showcode',
                            id: 'showcode',
                            width: 84,
                            margin: '0 0 0 0',
                            submitValue: true,
                            text: "<span id='key' style='font-size:12px;font-weight:bold;font-style:italic;'>" + returnCode() + "</span>",
                            listeners: {
                                click: function () {
                                    Ext.getCmp('showcode').setText("<span id='key' style='font-size:12px;font-weight:bold;font-style:italic;'>" + returnCode() + "</span>");
                                }
                            }
                        }
                     ]
                 }
        ],
        buttons: [{
            text: "驗證",
            formBind: true,
            id: 'btnSub',
            //vtype: 'submit',
            disabled: true,
            listeners: {
                click: function () {
                    this.disable();
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        var oVal = Ext.getCmp('check_code');
                        if (oVal.getValue() != "") {
                            var reg = /\s/g;
                            if (document.getElementById("key").innerHTML.replace(reg, '') != oVal.getValue()) {
                                Ext.Msg.alert(INFORMATION, "驗證碼錯誤！");
                                oVal.setValue("");
                                Ext.getCmp('showcode').fireEvent('click');
                                return false;
                            }
                        }
                        if (Ext.htmlEncode(Ext.getCmp('oldpassword').getValue()) == Ext.htmlEncode(Ext.getCmp('password').getValue()) && Ext.htmlEncode(Ext.getCmp('oldpassword').getValue()) != "") {
                            Ext.Msg.alert(INFORMATION, "資安新密碼不可以等於原始密碼！");
                            Ext.getCmp('password').setValue("");
                            return;
                        }
                        form.submit({
                            params: {
                                //裡面的數據錯誤。會導致無法顯示各種數據
                                password: Ext.htmlEncode(Ext.getCmp('password').getValue()),
                                oldpassword: Ext.htmlEncode(Ext.getCmp('oldpassword').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    if (result.error == 0) {
                                        boolPassword = false;
                                        WinSec.close();
                                    }
                                    else if (result.error == 1 || result.error == 5) {
                                        if (result.count == 0) {
                                            Ext.Msg.alert(INFORMATION, "警告：密碼輸入錯誤次數已達上限,請聯繫管理員！");
                                            isShow = false;
                                            isEdit = false;
                                            WinSec.close();
                                        }
                                        else {
                                            if (result.error == 5) {
                                                Ext.Msg.alert(INFORMATION, "警告：原始密碼輸入錯誤！還有" + result.count + "次機會！");
                                            } else {
                                                Ext.Msg.alert(INFORMATION, "資安密碼錯誤，還有" + result.count + "次機會！");
                                            }
                                            Ext.getCmp('password').setValue("");
                                            Ext.getCmp('check_code').setValue("");
                                            Ext.getCmp('showcode').fireEvent('click');
                                        }
                                    }
                                    else if (result.error == 2) {
                                        Ext.Msg.alert(INFORMATION, "警告：您沒有資安權限！");
                                    }
                                    else if (result.error == 3) {
                                        Ext.Msg.alert(INFORMATION, "對不起，後台未獲取到您的密碼！");
                                    }
                                    else if (result.error == 4) {
                                        Ext.Msg.alert(INFORMATION, "警告：您的查詢次數已達上限,請聯繫管理員！");
                                        isShow = false;
                                        isEdit = false;
                                        WinSec.close();
                                    }
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                }
            }
        }]

    });
    var WinSec = Ext.create('Ext.window.Window', {
        title: "資安驗證",
        iconCls: 'icon-user-edit',
        id: 'WinSec',
        width: 400,
        height: 175,
        layout: 'fit',
        items: [FrmSec],
        y: 200,
        closeAction: 'destroy',
        modal: true,
        constrain: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('WinSec').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                FrmSec.getForm().reset(); //如果是添加的話
            },
            'close': function () {
                if (isLogin && isShow) {//彈出驗證框，關閉時彈出顯示框
                    WinShow.show();
                }
                if (isEdit) {
                    if (urlType == "/Manage/ManageUser/changePwd ") {//彈出修改密碼窗口
                        ChangePwdFunction(relatedID);
                    }
                    else if (urlType == "/Member/UsersListIndex/EditEmail") {
                        editEmailFunction(relatedID, edit_UserStore);
                    }
                    else if (urlType == "/OrderManage/ModifyDeliverData") {
                        modifyDeliverData();
                    }
                    else if (urlType.substring(0, 21) == "/Vendor/VendorDetails") {
                        showDetail(relatedID);
                    }
                    else if (urlType.substring(0, 43) == "http://www.gigade100.com/ecservice_jump.php") {
                        TranToDetial(relatedID);
                    }
                    else {
                        editFunction(relatedID);
                    }
                }
            }
        }
    });
    if (isLogin) {//是否彈出驗證框
        WinSec.show();
    }
    var FrmShow = Ext.create('Ext.form.Panel', {
        id: 'FrmShow',
        frame: true,
        autoScroll: true,
        defaultType: 'displayfield',
        layout: 'anchor',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                fieldLabel: '客戶編號',
                id: 'user_id',
                name: 'user_id',
                hidden: true
            },
            {
                fieldLabel: '客戶名稱',
                id: 'user_name',
                name: 'user_name',
                hidden: true
            },
            {
                fieldLabel: '客戶簡稱',
                id: 'user_name_simple',
                name: 'user_name_simple',
                hidden: true
            },
            {
                fieldLabel: '客戶郵箱',
                id: 'user_email',
                name: 'user_email',
                hidden: true
            },
            {
                fieldLabel: '客戶電話',
                id: 'user_phone',
                name: 'user_phone',
                hidden: true
            },
            {
                fieldLabel: '客戶電話',
                id: 'user_mobile',
                name: 'user_mobile',
                hidden: true
            },
            {
                fieldLabel: '客戶地址',
                id: 'user_adress',
                name: 'user_adress',
                hidden: true
            },
            {
                fieldLabel: '客服代下單',
                id: 'user_proxy',
                name: 'user_proxy',
                hidden: true
            },
            {
                fieldLabel: '付款單號',
                id: 'u_order',
                name: 'u_order',
                hidden: true
            },
            {
                fieldLabel: '被推薦人',
                id: 'ur_name',
                name: 'ur_name',
                hidden: true
            },
            {
                fieldLabel: '郵箱',
                id: 'ur_mail',
                name: 'ur_mail',
                hidden: true
            },
            {
                fieldLabel: '推薦人',
                id: 'no_name',
                name: 'no_name',
                hidden: true
            },
            {
                fieldLabel: '發件者設定',
                id: 'sender_address',
                name: 'sender_address',
                hidden: true
            },
            {
                fieldLabel: '發件者姓名',
                id: 'sender_name',
                name: 'sender_name',
                hidden: true
            },
            {
                fieldLabel: '發件類型',
                id: 'send_type',
                name: 'send_type',
                hidden: true
            },
            {
                fieldLabel: '收件者設定',
                id: 'recipient',
                name: 'recipient',
                hidden: true
            }, {
                fieldLabel: '收件者姓名',
                id: 'recipient_name',
                name: 'recipient_name',
                hidden: true
            },
            {
                fieldLabel: '福委姓名',
                id: 'group_committe_chairman',
                name: 'group_committe_chairman',
                hidden: true
            },
            {
                fieldLabel: '福委電話',
                id: 'group_committe_phone',
                name: 'group_committe_phone',
                hidden: true
            },
            {
                fieldLabel: '福委mail',
                id: 'group_committe_mail',
                name: 'group_committe_mail',
                hidden: true
            },
            {
                fieldLabel: '查詢ID',
                id: 'order_id',
                name: 'order_id',
                hidden: true
            },
            {
                fieldLabel: '訂購姓名',
                id: 'order_name',
                name: 'order_name',
                hidden: true
            },
            {
                fieldLabel: '購買人姓名',
                id: 'order_name',
                name: 'order_name',
                hidden: true
            },
            {
                fieldLabel: '收貨人姓名',
                id: 'delivery_name',
                name: 'delivery_name',
                hidden: true
            },
            {
                fieldLabel: '購買人市內電話',
                id: 'order_phone',
                name: 'order_phone',
                hidden: true
            },
            {
                fieldLabel: '收貨人市內電話',
                id: 'delivery_phone',
                name: 'delivery_phone',
                hidden: true
            },
            {
                fieldLabel: '購買人手機',
                id: 'order_mobile',
                name: 'order_mobile',
                hidden: true
            },
            {
                fieldLabel: '收貨人手機',
                id: 'delivery_mobile',
                name: 'delivery_mobile',
                hidden: true
            },
            {
                fieldLabel: '帳單地址',
                id: 'order_address',
                name: 'order_address',
                hidden: true
            },
            {
                fieldLabel: '收貨地址',
                id: 'delivery_address',
                name: 'delivery_address',
                hidden: true
            },
            {
                fieldLabel: '查詢ID',
                id: 'id',
                name: 'id',
                hidden: true
            },
            {
                fieldLabel: '交易卡號',
                id: 'pan',
                name: 'pan',
                hidden: true
            },
            {
                fieldLabel: '發卡銀行',
                id: 'bankname',
                name: 'bankname',
                hidden: true
            },
            {
                fieldLabel: '查詢ID',
                id: 'nccc_id',
                name: 'nccc_id',
                hidden: true
            },
            {
                fieldLabel: '交易卡號',
                id: 'nccc_pan',
                name: 'nccc_pan',
                hidden: true
            },
            {
                fieldLabel: '髮卡銀行',
                id: 'nccc_bankname',
                name: 'nccc_bankname',
                hidden: true
            }
        ]

    });
    var WinShow = Ext.create('Ext.window.Window', {
        title: "客戶機敏信息",
        iconCls: 'ui-icon-unlocked',
        id: 'WinShow',
        width: 400,
        layout: 'fit',
        items: [FrmShow],
        y: 200,
        height: 200,
        closeAction: 'destroy',
        modal: true,
        constrain: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false
        ,
        tools: [
         {
             type: 'close',
             qtip: "關閉",
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('WinShow').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }
        ],
        listeners: {
            'show': function () {
                var mail = ""; var name = ""; var ur_name = ""; var ur_mail = ""; var no_ur_name = "";
                var infoarr = "";
                if (secret_info != null) {
                    infoarr = secret_info.split(";");
                }
                Ext.Ajax.request({
                    url: '/SecretInfo/GetUserInfo',//根據relatedID得到客戶信息
                    method: 'post',
                    async: false,
                    params: {
                        type: type,
                        urlType: urlType,
                        relatedID: relatedID,
                        info_id: info_id,
                        info_type: info_type
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            if (infoarr.indexOf("user_id") >= 0 || infoarr == "") {
                                if (result.user_id != "" && result.user_id != "0" && result.user_id != undefined) {
                                    Ext.getCmp('user_id').setValue(result.user_id).show();
                                }
                            }
                            if (infoarr.indexOf("user_name") >= 0 || infoarr == "") {
                                if (result.user_name != "" && result.user_name != undefined) {
                                    //  Ext.getCmp('user_name').setValue(result.user_name).show();
                                    name = result.user_name;
                                }
                            }
                            if (infoarr.indexOf("user_email") >= 0 || infoarr == "") {
                                if (result.user_email != "" && result.user_email != undefined) {
                                    mail = result.user_email;
                                }
                            }
                            //mail = result.user_email;
                            if (infoarr.indexOf("user_mobile") >= 0 || infoarr == "") {
                                if (result.user_mobile != "" && result.user_mobile != undefined) {
                                    Ext.getCmp('user_mobile').setValue(result.user_mobile).show();
                                }
                            }

                            if (infoarr.indexOf("user_phone") >= 0 || infoarr == "") {
                                if (result.user_phone != "" && result.user_phone != undefined) {
                                    Ext.getCmp('user_phone').setValue(result.user_phone).show();
                                }
                            }
                            if (infoarr.indexOf("user_adress") >= 0 || infoarr == "") {
                                if (result.user_adress != "" && result.user_adress != undefined) {
                                    Ext.getCmp('user_adress').setValue(result.user_adress).show();
                                }
                            }
                            if (result.simple_name != undefined) {//供應商簡稱
                                Ext.getCmp('user_name_simple').setValue(result.simple_name).show();
                            }
                            if (result.order_id != undefined) {//供應商簡稱
                                // Ext.getCmp('u_order').setValue(result.order_id).show();
                                Ext.getCmp('u_order').setValue(Ext.String.format("<a id='order_id' onclick='SaveRecord({0},4)' href='javascript:void(0);' >{1}</a>", relatedID, result.order_id)).show();//

                            }
                            if (result.ur_name != undefined) {//供應商簡稱
                                // Ext.getCmp('u_order').setValue(result.order_id).show();
                                // Ext.getCmp('u_order').setValue(Ext.String.format("<a id='order_id' onclick='SaveRecord({0},4)' href='javascript:void(0);' >{1}</a>", relatedID, result.order_id)).show();//
                                ur_name = result.ur_name;
                            }
                            if (result.ur_mail != undefined) {//供應商簡稱
                                // Ext.getCmp('u_order').setValue(result.order_id).show();
                                // Ext.getCmp('u_order').setValue(Ext.String.format("<a id='order_id' onclick='SaveRecord({0},4)' href='javascript:void(0);' >{1}</a>", relatedID, result.order_id)).show();//
                                ur_mail = result.ur_mail;
                            }
                            if (result.no_ur_name != undefined) {//供應商簡稱
                                // Ext.getCmp('u_order').setValue(result.order_id).show();
                                // Ext.getCmp('u_order').setValue(Ext.String.format("<a id='order_id' onclick='SaveRecord({0},4)' href='javascript:void(0);' >{1}</a>", relatedID, result.order_id)).show();//
                                no_ur_name = result.no_ur_name;
                            }
                            if (infoarr.indexOf("sender_address") >= 0 || infoarr == "") {
                                if (result.sender_address != "" && result.sender_address != undefined) {
                                    Ext.getCmp('sender_address').setValue(result.sender_address).show();;
                                }
                            }
                            //mail = result.user_email;
                            if (infoarr.indexOf("sender_name") >= 0 || infoarr == "") {
                                if (result.sender_name != "" && result.sender_name != undefined) {
                                    Ext.getCmp('sender_name').setValue(result.sender_name).show();
                                }
                            }

                            if (infoarr.indexOf("send_type") >= 0 || infoarr == "") {
                                if (result.send_type != "" && result.send_type != undefined) {
                                    if (result.send_type == 1) {
                                        Ext.getCmp('send_type').setValue("單人郵件").show();
                                    } else {
                                        Ext.getCmp('send_type').setValue("群組郵件").show();
                                    }

                                }
                            }
                            if (infoarr.indexOf("recipient") >= 0 || infoarr == "") {
                                if (result.recipient != "" && result.recipient != undefined) {
                                    Ext.getCmp('recipient').setValue(result.recipient).show();
                                }
                            }
                            if (infoarr.indexOf("recipient_name") >= 0 || infoarr == "") {
                                if (result.recipient_name != "" && result.recipient_name != undefined) {
                                    Ext.getCmp('recipient_name').setValue(result.recipient_name).show();
                                }
                            }

                            if (result.order_id != undefined) {//供應商簡稱
                                 Ext.getCmp('order_name').setValue(result.order_name).show();                              
                            }

                            //if (infoarr.indexOf("group_committe_chairman") >= 0 || infoarr == "") {
                            //    if (result.group_committe_chairman != "" && result.group_committe_chairman != "0" && result.group_committe_chairman != undefined) {
                            //        Ext.getCmp('group_committe_chairman').setValue(result.group_committe_chairman).show();
                            //    }
                            //}
                            //if (infoarr.indexOf("group_committe_phone") >= 0 || infoarr == "") {
                            //    if (result.group_committe_phone != "" && result.group_committe_phone != undefined) {
                            //        Ext.getCmp('group_committe_phone').setValue(result.group_committe_phone).show();
                            //    }
                            //}
                            //if (infoarr.indexOf("group_committe_mail") >= 0 || infoarr == "") {
                            //    if (result.group_committe_mail != "" && result.group_committe_mail != undefined) {
                            //        Ext.getCmp('group_committe_mail').setValue(result.group_committe_mail).show();
                            //    }
                            //}
                            if (result.order_phone != undefined) {//
                                Ext.getCmp('order_phone').setValue(result.order_phone).show();
                            }
                            if (result.order_mobile != undefined) {//;
                                Ext.getCmp('order_mobile').setValue(result.order_mobile).show();
                            }
                            if (result.order_address != undefined) {//;
                                Ext.getCmp('order_address').setValue(result.order_address).show();
                            }
                            if (result.delivery_name != undefined) {//;
                                Ext.getCmp('delivery_name').setValue(result.delivery_name).show();
                            }
                            if (result.delivery_phone != undefined) {//;
                                Ext.getCmp('delivery_phone').setValue(result.delivery_phone).show();
                            }
                            if (result.delivery_mobile != undefined) {//;
                                Ext.getCmp('delivery_mobile').setValue(result.delivery_mobile).show();
                            }
                            if (result.delivery_address != undefined) {//
                                Ext.getCmp('delivery_address').setValue(result.delivery_address).show();
                            }
                            if (result.id != undefined) {//網際威信-ID
                                Ext.getCmp('id').setValue(result.id).show();
                            }
                            if (result.pan != undefined) {//網際威信-卡號
                                Ext.getCmp('pan').setValue(result.pan).show();
                            }
                            if (result.bankname != undefined) {//網際威信-髮卡銀行
                                Ext.getCmp('bankname').setValue(result.bankname).show();
                            }
                            if (result.nccc_id != undefined) {//聯合信用卡-ID
                                Ext.getCmp('nccc_id').setValue(result.nccc_id).show();
                            }
                            if (result.nccc_pan != undefined) {//聯合信用卡-卡號
                                Ext.getCmp('nccc_pan').setValue(result.nccc_pan).show();
                            }
                            if (result.nccc_bankname != undefined) {//聯合信用卡-髮卡銀行
                                Ext.getCmp('nccc_bankname').setValue(result.nccc_bankname).show();
                            }
                        } else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
                //type；1，會員2.訂單3.簡訊。4聯絡客服
                switch (type) {
                    case "1":
                        if (urlType == "/Member/RecommendMember")//推薦會員中推薦者的信息
                        {
                            if (ur_name != "") {
                                Ext.getCmp('ur_name').setValue(ur_name).show();
                            }
                            if (ur_mail != "") {
                                Ext.getCmp('ur_mail').setValue(ur_mail).show();
                            }
                            if (no_ur_name != "") {
                                Ext.getCmp('no_name').setValue(no_ur_name).show();
                            }
                        }                      
                        else {
                            if (urlType == "/Member/UsersListIndex") {
                                Ext.getCmp('user_email').setValue(Ext.String.format("<a id='u_mail'  onclick='SaveRecord({0},2)' href='' target='_blank'>{1}</a>", relatedID, mail)).show();//
                                Ext.getCmp('user_proxy').setValue(Ext.String.format("<a  onclick='SaveRecord({0},1)' href='' id='u_proxy' target='_blank'>進入</a>", relatedID)).show();
                            }
                            else {
                                if (mail != "") {
                                    Ext.getCmp('user_email').setValue(mail).show();
                                }
                            }
                            if (name != "") {
                                Ext.getCmp('user_name').setValue(name).show();
                            }
                        }
                        break;
                    case "2":
                        if (urlType == "/OrderManage/BrandProductIndex") {
                            if (result.order_id != "") {
                                Ext.getCmp('u_order').setValue(result.order_id).show();
                            }
                            if (result.order_name != "") {
                                Ext.getCmp('order_name').setValue(result.order_name).show();
                            }
                        }
                        else {
                            if (mail != "") {
                                Ext.getCmp('user_email').setValue(mail).show();
                            }
                            if (name != "") {
                                Ext.getCmp('user_name').setValue(name).show();
                            }
                        }
                        break;
                    case "3":
                        Ext.getCmp('user_phone').setValue(Ext.String.format("<a id='u_phone'  href=javascript:TransToUser(\"{0}\") >{0}</a>", Ext.getCmp('user_phone').getValue()));
                        break;
                    case "7":
                        if (urlType == "/Vendor/VendorList") {
                            if (name != "") {
                                Ext.getCmp('user_name').setValue(Ext.String.format("<a id='u_name'  href='javascript:void(0);' onclick='SaveRecord({0},3)' >{1}</a>", relatedID, name)).show();
                            }
                            else {
                                Ext.getCmp('user_name').setValue(name).show();
                            }
                        }
                        break;
                    case "15":  //訊息管理
                        if (urlType == "/Edm/EdmPersonList") {
                            if (name != "") {
                                Ext.getCmp('user_name').setValue(Ext.String.format("<a id='u_name'  href='javascript:void(0);' onclick='SaveRecord({0},15)' >{1}</a>", relatedID, name)).show();
                            }
                            if (mail != "") {
                                Ext.getCmp('user_email').setValue(mail).show();
                            }
                        } else {
                            if (mail != "") {
                                Ext.getCmp('user_email').setValue(mail).show();
                            }
                            if (name != "") {
                                Ext.getCmp('user_name').setValue(name).show();
                            }
                        }
                        break;
                    case "18": 
                        if (urlType == "/VipUserGroup/Index ") {
                            SaveRecord(relatedID, 18);
                        }
                        else {
                            if (mail != "") {
                                Ext.getCmp('user_email').setValue(mail).show();
                            }
                            if (name != "") {
                                Ext.getCmp('user_name').setValue(name).show();
                            }
                        }
                        break;                   
                    default:
                        if (mail != "") {
                            Ext.getCmp('user_email').setValue(mail).show();
                        }
                        if (name != "") {
                            Ext.getCmp('user_name').setValue(name).show();
                        }
                        break;
                }
            }
        }
    });
    if (isShow && (!isLogin) && pwd_status == 1) {//不彈驗證框，直接顯示信息
        if (type == "18" && urlType == "/VipUserGroup/Index ") {
            SaveRecord(relatedID, 18);
        }
        else {
            WinShow.show();
        }
    }
    if (pwd_status == 0) {
        WinSec.show();
    }

}

//返回四位隨機整數驗證碼
function returnCode() {
    var rnd = "";
    for (var i = 0; i < 4; i++) {
        rnd += Math.floor(Math.random() * 10) + " ";//獲取0-9的隨機整數
    }
    return rnd;
}

function SaveRecord(rID, type, win) {
    var obj; var url; var link;
    if (type == "1") {//客服代下單
        url = "/Member/UsersListIndex/客服代下單 ";
        obj = document.getElementById('u_proxy');
        link = "http://www.gigade100.com//ecservice_jump.php?uid=" + rID;
    } else if (type == "2") {//發郵件
        url = "/Member/UsersListIndex/發送郵件";
        obj = document.getElementById('u_mail');
        link = "mailto:" + obj.innerHTML;
    }
    else if (type == "3") {
        url = "/Vendor/VendorList/供應商聯繫人"
        SaveSecretLog(url, "7", rID);
        VendorFunction(rID);
        Ext.getCmp('WinShow').destroy();
        return false;
    }
    else if (type == "4") {
        var orderId = document.getElementById('order_id').innerHTML;
        url = '/OrderManage/OrderDetialList?Order_Id=' + orderId;
        SaveSecretLog(url, "2", rID);
        TransToOrder(orderId);
        return false;
    }
    else if (type == "15") {//發信名單統計
        var email_id = rID;
        url = '/Edm/SendRecordList?eid=' + email_id;
        SaveSecretLog(url, "15", email_id);
        TranToDetial(email_id);
        return false;
    }
    else if (type == "18") {
        url = "/VipUserGroup/list/企業用戶管理"
        SaveSecretLog(url, "7", rID);
        GCCInfoFunction(rID);
        Ext.getCmp('WinShow').destroy();
        return false;
    }
    boolPassword = SaveSecretLog(url, "1", rID);
    if (boolPassword != "-1") {
        obj.href = link;
        return true;
    }
    else {
        return false;
    }
}
//參數不為空時插入用戶查詢記錄 ，參數為空時查詢5分鐘內是否有輸入密碼
function SaveSecretLog(urlRecord, secretType, ralatedId) {
    var boolPassword = true;
    var isConti = true;
    if (urlRecord != null && secretType != null && ralatedId != null) {

        Ext.Ajax.request({
            url: '/SecretInfo/SaveSecretLog',
            method: 'post',
            async: false,
            params: {
                urlRecord: urlRecord,
                secretType: secretType,
                ralatedId: ralatedId
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    if (result.ispower) {//isconti,ispower是否有權限
                        if (!result.isconti) {
                            Ext.Msg.alert(INFORMATION, "警告：您的查詢次數已達上限,請聯繫管理員！");
                            isConti = false;
                            boolPassword = "-1";//不執行後續操作
                        } else {
                            pwd_status = result.pwd_status
                            if (pwd_status == 0) {
                                isConti = false;
                                boolPassword = true;
                            }
                        }
                    }
                    else {
                        Ext.Msg.alert(INFORMATION, "警告：您沒有資安權限！");
                        isConti = false;
                        boolPassword = "-1";
                    }

                } else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
    if (isConti) {
        //查看是否已經輸入資安密碼，若已經輸入且不超過5分中，則直接顯示機敏信息，超過5分中則星號顯示機敏信息
        Ext.Ajax.request({
            url: '/SecretInfo/GetSecretLog',
            method: 'post',
            async: false,
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    if (!result.data) {
                        boolPassword = false;
                    }
                } else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
    return boolPassword;
}

