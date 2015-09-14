

Ext.onReady(function () {
    var addTab = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 900,
        url: '/WareHouse/GETMarkTallyWD',
        defaults: {
            labelWidth: 100,
            width: 250,
            margin: '5 0 0 10'
        },
        border: false,
        plain: true,
        id: 'AddUserPForm',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '編號',
                id: 'icd_id',
                name: 'icd_id',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '商品細項編號',
                id: 'item_id',
                name: 'item_id',
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '原料位編號',
                id: 'icd_old_loc_id',
                name: 'icd_old_loc_id'
            },
             {
                 xtype: "textfield",
                 id: 'old_loc_id',
                 name: 'old_loc_id',
                 fieldLabel: '料位編號',
                 submitValue: true,
                 listeners: {
                     change: function () {
                         var old_loc = Ext.getCmp("old_loc_id").getValue();
                         var old_loc_id = Ext.getCmp("icd_old_loc_id").getValue();
                         
                         if (old_loc == old_loc_id) {
                             Ext.getCmp("error1").hide();
                             Ext.getCmp("product_name").show();
                             Ext.getCmp("iupc_item").show();

                         }
                         else {
                             if (old_loc.length > 7) {
                                 Ext.getCmp("error1").show();
                                 Ext.getCmp("error1").setValue("<div style='color:red;'>商品原料位編號輸入錯誤,請重新輸入!</div>");
                                 Ext.getCmp("old_loc_id").setValue();
                                 Ext.getCmp("old_loc_id").focus();
                             



                                 Ext.getCmp("product_name").hide();
                                 Ext.getCmp("iupc_item").hide();
                                 Ext.getCmp("iupc_item").setValue();
                                 Ext.getCmp("icd_new_loc_id").hide();
                                 Ext.getCmp("new_loc_id").hide();
                                 Ext.getCmp("new_loc_id").setValue();
                                 Ext.getCmp("btn_sure").hide();

                             } else
                             {
                                 Ext.getCmp("product_name").hide();
                                 Ext.getCmp("iupc_item").hide();
                                 Ext.getCmp("iupc_item").setValue();
                                 Ext.getCmp("icd_new_loc_id").hide();
                                 Ext.getCmp("new_loc_id").hide();
                                 Ext.getCmp("new_loc_id").setValue();
                                 Ext.getCmp("btn_sure").hide();
                             }
                            
                         }
                     }
                 }
            
                
             },
               {
                   xtype: 'displayfield',
                   fieldLabel: '提示',
                   id: 'error1',
                   name: 'error1',
                   style: 'color:red;',
                   width: 600,
                   hidden: true
               },
              {
                  xtype: 'displayfield',
                  fieldLabel: '品名',
                  id: 'product_name',
                  name: 'product_name',
                  hidden: true
                 
              },
            {
                xtype: 'textfield',
                fieldLabel: '條碼/商品編號',
                id: 'iupc_item',
                name: 'iupc_item',
                hidden: true,
                listeners: {
                    change: function () {
                        //判斷該商品item對應的條碼
                        if (Ext.getCmp('iupc_item').getValue().length > 5) {
                            Ext.Ajax.request({
                                url: "/WareHouse/Judgeupcid",
                                method: 'post',
                                type: 'text',
                                params: {
                                    item_id: Ext.getCmp("item_id").getValue(),
                                    upc_id: Ext.getCmp("iupc_item").getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.getCmp("error2").hide();
                                        Ext.getCmp("icd_new_loc_id").show();
                                        Ext.getCmp("new_loc_id").show();
                                       
                                    } else {
                                        if (result.msg == 1) {
                                            Ext.getCmp("error2").setValue("<div style='color:red;'>這個條碼對應到" + result.itemid + "品號，請重新掃描正確商品!</div>");
                                        }
                                        else if (result.msg == 2) {
                                            Ext.getCmp("error2").setValue("<div style='color:red;'>這個條碼不存在，請重新掃描!</div>");
                                        }
                                        Ext.getCmp("error2").show();
                                       
                                

                                        Ext.getCmp("icd_new_loc_id").hide();
                                        Ext.getCmp("new_loc_id").hide();
                                        Ext.getCmp("new_loc_id").setValue();
                                        Ext.getCmp("btn_sure").hide();
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "條碼錯誤,請稍后再試,如有重複出錯請聯繫管理員!!");
                                    Ext.getCmp("iupc_item").focus();
                                }
                            });
                        } else {
                            Ext.getCmp("icd_new_loc_id").hide();
                            Ext.getCmp("new_loc_id").hide();
                            Ext.getCmp("new_loc_id").setValue();
                            Ext.getCmp("btn_sure").hide();
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: '提示',
                id: 'error2',
                name: 'error2',
                style: 'color:red;',
                width: 600,
                hidden: true
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: '新料位編號',
                 id: 'icd_new_loc_id',
                 name: 'icd_new_loc_id',
                 hidden: true

             },
            {
                xtype: 'textfield',
                fieldLabel: '料位編號',
                id: 'new_loc_id',
                name: 'new_loc_id',
                hidden: true,
                listeners: {
                    change: function () {
                        var new_loc = Ext.getCmp("icd_new_loc_id").getValue();
                        var new_loc_id = Ext.getCmp("new_loc_id").getValue();

                        if (new_loc == new_loc_id) {
                            Ext.getCmp("error3").hide();
                            Ext.getCmp("btn_sure").show();
                        }
                        else {
                            if (new_loc_id.length > 7) {
                                Ext.getCmp("error3").show();
                                Ext.getCmp("error3").setValue("<div style='color:red;'>商品新料位編號輸入錯誤,請重新輸入!</div>");
                                Ext.getCmp("new_loc_id").setValue();
                                Ext.getCmp("new_loc_id").focus();
                                Ext.getCmp("btn_sure").hide();
                            } else
                            {
                                Ext.getCmp("btn_sure").hide();
                            }

                        }
                    }
                }

            },
            {
                xtype: 'displayfield',
                fieldLabel: '提示',
                id: 'error3',
                name: 'error3',
                style: 'color:red;',
                width: 600,
                hidden: true
            }
        ],
        buttonAlign: 'left',
        buttons: [
            {
                id: 'btn_sure',
                text: '確認',
                hidden: true,
                formBind: true,
                disabled: true,
                handler: function () {
                    Ext.Ajax.request({
                        url: "/WareHouse/GetIlocChangeDetailEdit",
                        method: 'post',
                        type: 'text',
                        params: {
                            icd_id_In: Ext.htmlEncode(Ext.getCmp('icd_id').getValue())
                        }, 
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                //if (result.msg != undefined) {
                                //    Ext.Msg.alert(INFORMATION, result.msg);
                                //}
                                //else {
                                Ext.Msg.alert(INFORMATION, "操作成功!");
                                setTimeout('closedthis()',4000)

                               
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, "操作失敗!");
                                // IlocChangeDetailsStore.load();
                                
                            }
                        },
                        failure: function (form, action) {

                            Ext.Msg.alert(INFORMATION, "操作超時!");

                        }
                    });
                }
            }
        ],
        listeners: {
            afterrender: function () {
                //獲取工作代號用於顯示并連帶出相關信息
                var icd = document.getElementById("Icd_Id").value;
                Ext.getCmp("icd_id").setValue(icd);
                LoadWorkInfo(icd);
            }
        }
    });
    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: addTab,
        autoScroll: true
    });

});

function closedthis()
{
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#IlocChangeDetailLink');
    copy.close();
}
//初始化頁面加載信息
function LoadWorkInfo(icd) {

    Ext.Ajax.request({
        url: '/WareHouse/MaterialHandling',
        method: 'post',
        params: {
            icd: icd
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                //請求成功,控制下一步的操作
                if (result.data.length > 0) {
                   
                    Ext.getCmp("icd_old_loc_id").setValue(result.data[0].icd_old_loc_id);
                    var product = '[' + result.data[0].icd_item_id + ']' + result.data[0].product_name + ' ' + result.data[0].product_sz;
                    Ext.getCmp("product_name").setValue(product);
                    Ext.getCmp("icd_new_loc_id").setValue(result.data[0].icd_new_loc_id);
                    Ext.getCmp("item_id").setValue(result.data[0].icd_item_id);
                   
                   
                } 
            }
        },
        failure: function () {
            //Ext.Msg.alert("提示","您輸入的料位條碼不符,請查找對應的料位!");
        }
    });
}

