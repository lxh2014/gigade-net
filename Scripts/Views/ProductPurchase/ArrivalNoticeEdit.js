
function editFunction(rowID,row) {
   
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        autoScroll: true,
        layout: 'anchor',
        url: '/ProductPurchase/SaveArrivaleNotice',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: "displayfield",
                fieldLabel: "取消編號",
                id: 'id',
                name: 'id',
                allowBlank: false,
                value:0,
               hidden: true

            },
                {
                    xtype: "textfield",
                    fieldLabel: "會員編號",
                    id: 'user_id',
                    name: 'user_id',
                    allowBlank: false,
                    regex: /^[1-9]\d*$/,
                    maxLength:9,
                    listeners: {
                        'blur': function () {////'blur'
                            //var us = Ext.getCmp("user_id");
                            var reg = /^[1-9]\d*$/;
                            if (reg.test(Ext.getCmp("user_id").getValue())) {


                                //if (us.getValue().isValid()) {
                                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "請稍等......", removeMask: true });
                                Ext.Ajax.request({
                                    url: "/ProductPurchase/GetUserName",
                                    params: {
                                        //user_id: Ext.htmlEncode(document.getElementById("hid_item_id").value),
                                        user_id: Ext.getCmp("user_id").getValue()
                                    },
                                    success: function (response) {
                                        myMask.hide();
                                        var result = Ext.decode(response.responseText);
                                        if (result.msg == "100") {
                                            Ext.getCmp("user_name").setValue('<font style="color:red">沒有此會員信息</font>');
                                            Ext.getCmp("users").setValue(0);
                                        }
                                        else {
                                            Ext.getCmp("user_name").setValue(result.msg);
                                            Ext.getCmp("users").setValue(Ext.getCmp("user_id").getValue());
                                        }
                                    },
                                    failure: function (form, action) {
                                        myMask.hide();
                                        Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                                    }
                                });
                            } else
                            {
                                Ext.getCmp("user_name").setValue('<font style="color:red">沒有此會員信息</font>');
                                Ext.getCmp("users").setValue(0);
                            }
                          
                        }
                    }

                },
                 {
                     xtype: "displayfield",
                     fieldLabel: "會員真實編號",
                     id: 'users',
                     name: 'users',
                     allowBlank: false,
                     hidden: true

                 },
                 {
                     xtype: "displayfield",
                     fieldLabel: "會員名稱",
                     id: 'user_name',
                     name: 'user_name',
                     allowBlank: false
                 },
                {
                    xtype: 'textfield',
                    fieldLabel: '商品細項編號',
                    id: 'item_id',
                    regex: /^[1-9]\d*$/,
                    maxLength: 6,
                    name: 'item_id',
                    allowBlank:false,
                    listeners: {
                        change: function () {// change:
                            //判斷該商品item對應的信息
                            
                            var reg = /^[1-9]\d*$/;
                            if (reg.test(Ext.getCmp("item_id").getValue())) {

                                if (Ext.getCmp("item_id").getValue().length == 6) {
                                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "請稍等......", removeMask: true });
                                    Ext.Ajax.request({
                                        url: "/ProductPurchase/GetProductName",
                                        params: {
                                            item_id: Ext.getCmp("item_id").getValue()
                                        },
                                        success: function (response) {
                                            myMask.hide();
                                            var result = Ext.decode(response.responseText);
                                            if (result.msg == "100") {
                                                Ext.getCmp("product_name").setValue('<font style="color:red">沒有此商品信息</font>');
                                                Ext.getCmp("product_No").setValue(0);
                                            }
                                            else {
                                                Ext.getCmp("product_name").setValue(result.msg);
                                                Ext.getCmp("product_No").setValue(result.product_id);
                                            }

                                        },
                                        failure: function (form, action) {
                                            myMask.hide();
                                            Ext.Msg.alert(INFORMATION, "系統出現錯誤!");
                                        }
                                    });
                                }
                                else {
                                    Ext.getCmp("product_name").setValue('<font style="color:red">沒有此商品信息</font>');
                                    Ext.getCmp("product_No").setValue(0);
                                }
                            }
                        }
                    }
                   
                },
                 {
                     xtype: "displayfield",
                     fieldLabel: "商品編號",
                     id: 'product_No',
                     name: 'product_No',
                     allowBlank: false,
                     hidden:true

                 },
                {
                    xtype: "displayfield",
                    fieldLabel: "商品名稱",
                    id: 'product_name',
                    name: 'product_name',
                    allowBlank: false
                }

            
        ],
        buttons: [
              
            {
                formBind: true,
                disabled: true,
                text: "保存",
                id: 'btn_reply',
                vtype: 'submit',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (Ext.getCmp('users').getValue().trim() == "0" || Ext.getCmp('users').getValue().trim() == "" || Ext.getCmp('product_No').getValue() == "0" || Ext.getCmp('product_No').getValue() == "") {
                        Ext.Msg.alert(INFORMATION, "會員或商品信息填寫錯誤!");
                        return;
                    }
                    Ext.MessageBox.confirm(CONFIRM, "確定保存？", function (btn) {
                        if (btn == "yes") {
                            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "請稍等......",removeMask: true});
                            myMask.show();
                            if (form.isValid()) {
                                form.submit({
                                    //waitMsg: '提交回復中,請稍等......',
                                    //waitTitle: '提示',
                                    //timeout: 9000000,
                                    params: {
                                        id: Ext.getCmp('id').getValue(),
                                        product_id: Ext.getCmp('product_No').getValue(),
                                        item_id: Ext.getCmp('item_id').getValue(),
                                        user_id: Ext.getCmp('user_id').getValue()
                                    },
                                    success: function (form, action) {
                                        myMask.hide();
                                        var result = Ext.decode(action.response.responseText);
                                        if (result.success) {
                                          
                                            if (result.msg == "1") {
                                                Ext.Msg.alert("提示", "保存就失敗!");
                                            }
                                            else if (result.msg == "98") {
                                                Ext.Msg.alert("提示", "此人員已在未通知列表中!");
                                            }
                                            else if (result.msg == "100") {
                                                Ext.Msg.alert("提示", "此人不在補貨通知以內或已取消通知!");
                                            }
                                            else {
                                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                                row.load();
                                                editWins.close();
                                            }
                                        }
                                        else {
                                            Ext.Msg.alert(INFORMATION, FAILURE);
                                            row.load();
                                            editWins.close();
                                        }
                                    },
                                    failure: function (form, action) {
                                        myMask.hide();
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                        row.load();
                                        editWins.close();
                                    }
                                });
                            }
                        }
                        else {
                            return false;
                        }
                    });
                }
            }
        ]
    });
    var editWins = Ext.create('Ext.window.Window', {
        title: rowID ? "取消補貨通知" : "新增補貨通知",// "補貨通知統計",//
        iconCls: rowID ? "icon-user-edit" : "icon-user-add",
        id: 'editWins',
        width: 400,
        height:230,
        layout: 'fit',
        items: [editFrm],
        constrain: true,
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWins').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (rowID != null) {
                    initForm(rowID);
                }
            }
        }
    });
    editWins.show();
    function initForm(Row) {
        Ext.getCmp('id').setValue(Row);
    }
}
