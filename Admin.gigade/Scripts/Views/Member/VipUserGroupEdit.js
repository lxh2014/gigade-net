editFunction = function (row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        url: '/Member/SaveVipUserGroup',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'textfield',
            fieldLabel: '名單編號',
            id: 'group_id',
            name: 'group_id',
            submitValue: true,
            hidden: true
        },
        {
            xtype: 'textfield',
            name: 'group_name',
            id: 'group_name',
            submitValue: true,
            allowBlank: false,
            maxLength: 25,
            fieldLabel: '名單名稱'
        }, {
            fieldLabel: '統編',
            xtype: 'textfield',
            name: 'tax_id',
            id: 'tax_id',
            maxLength: 25,
            submitValue: false,
            allowBlank: true                      
        }, {
            fieldLabel: '企業商品類別',
            xtype: 'textfield',
            name: 'group_category',
            id: 'group_category',
            value: 0,
            maxLength: 9,
            submitValue: false,
            allowBlank: true      
        }, {
            fieldLabel: '企業英文名稱' ,
            xtype: 'textfield',
            name: 'eng_name',
            id: 'eng_name',
            allowBlank: true,
            maxLength: 10,
            submitValue: false,
            regex: /^[A-Za-z]+$/,
            regexText: '只能輸入英文',
            allowBlank: true       
        }, {
            xtype: 'textfield',
            name: 'gift_bonus',
            id: 'gift_bonus',
            allowBlank: true,
            minValue: 0,
            maxLength: 4,
            fieldLabel: '註冊贈送購物金額',
            readOnly: true,
            submitValue: true,
        },
        {
            xtype: 'displayfield',
            name: 'bonus_rate',
            id: 'bonus_rate',
            allowBlank: true,
            maxLength: 10,
            hidden: true,
            submitValue: true,
            readOnly:true,
            fieldLabel: '購物贈送購物金倍率'
        },
        {
            xtype: 'displayfield',
            name: 'bonus_expire_day',
            id: 'bonus_expire_day',
            allowBlank: true,
            maxLength: 10,
            hidden:true,
            submitValue: true,
            readOnly: true,
            fieldLabel: '購物贈送購物金有效天數'
        },
        {
            xtype: 'filefield',
            name: 'image_name',
            id: 'image_name',
            fieldLabel: '企業圖片',
            msgTarget: 'side',
            anchor: '100%',
            allowBlank: true,
            buttonText: '選擇',
            validator:
            function (value) {
                if (value != '') {
                    var type = value.split('.');
                    if (type[type.length - 1] == 'jpg' || type[type.length - 1] == 'gif' || type[type.length - 1] == 'png') {
                        return true;
                    }
                    else {
                        return '上傳文件類型不正確！';
                    }
                }
                else {
                    return true;
                }
            },
            fileUpload: true,
            submitValue: true
        }, {
            xtype: 'radiogroup',
            fieldLabel: '企業員工身份驗證',
            //id: 'validate',
            submitValue: true,                    
            items: [{
                boxLabel: '否',
                name: 'check_iden',
                inputValue: '0',
                id: 'radio1',
                checked: true ,
                listeners: {
                    change: function (radio, newValue, oldValue) {                                                            
                        if (newValue) {
                            Ext.getCmp("tax_id").allowBlank = true;
                            Ext.getCmp("group_category").allowBlank = true;
                            Ext.getCmp("eng_name").allowBlank = true;
                            Ext.getCmp('tax_id').getEl().first().dom.innerHTML = '統編';
                            Ext.getCmp('group_category').getEl().first().dom.innerHTML = '企業商品類別';
                            Ext.getCmp('eng_name').getEl().first().dom.innerHTML = '企業英文名稱';
                            Ext.getCmp("tax_id").reset();
                            Ext.getCmp("group_category").reset();
                            Ext.getCmp("eng_name").reset();
                        }
                    }
                }

            }, {
                boxLabel: '是',
                name: 'check_iden',
                inputValue: '1',
                id: 'radio2',
                listeners: {
                    change: function (radio, newValue, oldValue) {                    
                        if (newValue) {
                            Ext.getCmp("tax_id").allowBlank = false;                            
                            Ext.getCmp("group_category").allowBlank = false;
                            Ext.getCmp("eng_name").allowBlank = false;
                            Ext.getCmp('tax_id').getEl().first().dom.innerHTML = '統編' + ":<font style='color:red'>" +'*' +"</font>";
                            Ext.getCmp('group_category').getEl().first().dom.innerHTML = '企業商品類別' + ":<font style='color:red'>" + '*' + "</font>";
                            Ext.getCmp('eng_name').getEl().first().dom.innerHTML = '企業英文名稱' + ":<font style='color:red'>" + '*' + "</font>";
                            Ext.getCmp("tax_id").reset();
                            Ext.getCmp("group_category").reset();
                            Ext.getCmp("eng_name").reset();
                            //label = Ext.getCmp('no_discount').getEl().parent().parent().first();
                            //  label.dom.innerHTML = DISCOUNT+(100-newValue)+"%";
                            //Ext.DomQuery.selectNode('label[for=no_discount]').innerHTML =DISCOUNT+(100-newValue)+ '%';
                        }
                    }                 
                }
            }]
        }
        , {
            xtype: 'displayfield',
            name: 'info',
            id: 'info',
            readOnly: true,
            
            fieldLabel: "提示信息",
            value:"<font style='color:red'>" +  "*  為必填項" + "</font>"
        }],
        buttons: [{
            //formBind: true,
            //disabled: false,
            id:'saveBtn',
            text: '保存',
            handler: function () {
             var form = this.up('form').getForm();
                form.submit({
                    params: {
                        group_id: Ext.htmlEncode(Ext.getCmp('group_id').getValue()),
                        group_name: Ext.htmlEncode(Ext.getCmp('group_name').getValue()),
                        tax_id: Ext.htmlEncode(Ext.getCmp('tax_id').getValue()),
                        group_category: Ext.htmlEncode(Ext.getCmp('group_category').getValue()),
                        eng_name: Ext.htmlEncode(Ext.getCmp('eng_name').getValue()),
                        gift_bonus: Ext.htmlEncode(Ext.getCmp('gift_bonus').getValue()),
                        image_name: Ext.htmlEncode(Ext.getCmp('image_name').getValue()),
                        radio1: Ext.getCmp('radio1').getValue(),
                        radio2: Ext.getCmp('radio2').getValue()
                    },
                    success: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, "保存成功! " + result.msg);
                            VipUserGroupStore.load();
                            editWin.close();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "保存失敗! " + result.msg);
                            VipUserGroupStore.load();
                            editWin.close();
                        }
                    },
                    failure: function (form, action) {
                        var result = Ext.decode(action.response.responseText);
                        Ext.Msg.alert(INFORMATION, "保存失敗! " + result.msg);
                        VipUserGroupStore.load();
                        editWin.close();
                    }
                });

            }
            
        }]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: 'VIP名單新增/修改',
        iconCls: 'icon-user-edit',
        id: 'editWin',
        width: 400,
        y: 100,
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
             qtip: '是否關閉',
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('editWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }],
        listeners: {
            'show': function () {             
                if (row == null) {
                    if (document.getElementById("modify_only").value == 1) {
                        Ext.getCmp('bonus_rate').show();
                        Ext.getCmp('bonus_expire_day').show();
                    }
                    if (document.getElementById("valet_service").value == 1) {
                        Ext.getCmp('gift_bonus').setReadOnly(false);
                    }
                }
                else {
                    editFrm.getForm().loadRecord(row);
                    if (document.getElementById("modify_only").value == 1) {
                        Ext.getCmp('bonus_rate').show();
                        Ext.getCmp('bonus_expire_day').show();
                    }
                    if (document.getElementById("valet_service").value == 1) {
                        Ext.getCmp('gift_bonus').setReadOnly(false);
                    }
                }

            }
        }
    });
    editWin.show();
    initForm(row);
    
    
   
}
function initForm(row) {
    var img = row.data.image_name.toString();
    var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
    Ext.getCmp('image_name').setRawValue(imgUrl);
}




