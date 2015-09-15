
editFunction = function (Row, store) {
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Chinatrust/SaveChinaTrustBagMap',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: 'ID',
                id: 'map_id',
                name: 'map_id',
                hidden: true
            },
            {
                xtype: 'combobox',
                fieldLabel: "區域包",
                allowBlank: false,
                editable: false,
                hidden: false,
                id: 'bag_id',
                name: 'bag_id',
                lastQuery: '',
                store: DDRStore,
                displayField: 'bag_name',
                valueField: 'bag_id',
                typeAhead: true,
                forceSelection: false,
                labelWidth: 80,                
                value: document.getElementById('bag_id').value,
                emptyText: '請選擇'
            },
            {
                xtype: 'textfield',
                fieldLabel: "商品編號",
                labelWidth: 80,
                id: 'product_id',
                name: 'product_id',
                allowBlank: false,
                regex: /^[0-9]*$/,
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('product_id').getValue();
                        Ext.Ajax.request({//判斷是否為組合商品
                            url: "/Vote/GetProductType",
                            method: 'post',
                            type: 'text',
                            params: {
                                id: Ext.getCmp('product_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {
                                    msg = result.msg;
                                    if (msg == 0) {
                                        Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                        Ext.getCmp("product_id").setValue("");
                                    }
                                    else if (msg == 1) {//表示為單一商品
                                        Ext.Ajax.request({
                                            url: "/Vote/GetProductName",
                                            method: 'post',
                                            type: 'text',
                                            params: {
                                                id: Ext.getCmp('product_id').getValue()
                                            },
                                            success: function (form, action) {
                                                var result = Ext.decode(form.responseText);
                                                if (result.success) {
                                                    msg = result.msg;
                                                    if (msg == 0) {
                                                        Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                                        Ext.getCmp("product_id").setValue("");
                                                    }
                                                    else {
                                                        Ext.getCmp("product_name").setValue(msg);
                                                        Ext.Ajax.request({
                                                            url: "/Chinatrust/GetLink",
                                                            method: 'post',
                                                            type: 'text',
                                                            success: function (form, action) {
                                                                var result = Ext.decode(form.responseText);
                                                                if (result.success) {
                                                                    Ext.getCmp("linkurl").setValue(result.msg + id);
                                                                }
                                                            }
                                                        });
                                                    }
                                                }
                                                else {
                                                    Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                                    Ext.getCmp("product_id").setValue("");
                                                }
                                            }
                                        });
                                    }
                                    else {
                                        Ext.getCmp("product_name").setValue("不能為組合商品！");
                                        Ext.getCmp("product_id").setValue("");
                                    }
                                }
                                else {
                                    Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                    Ext.getCmp("product_id").setValue("");
                                }
                            }
                        });

                   
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: "品名",
                name: 'product_name',
                id: 'product_name'
            },
            {
                xtype: 'textfield',
                allowBlank: true,
                fieldLabel: "商品描述",
                labelWidth: 80,
                id: 'product_desc',
                name: 'product_desc'
            },
            {
                xtype: 'numberfield',
                fieldLabel: "商品排序",
                id: 'map_sort',
                name: 'map_sort',
                value: 0,
                minValue: 0,
                maxValue: 999999,
                allowDecimal: false,
                editable: false,
                hidden: true
            },
            {
                xtype: 'textfield',
                allowBlank: true,
                fieldLabel: "廣告商品編號",
                labelWidth: 80,
                id: 'ad_product_id',
                name: 'ad_product_id',
                regex: /^[0-9 ,]+$/
            },
            {
                xtype: 'textfield',
                allowBlank: false,
                fieldLabel: "商品鏈接地址",
                labelWidth: 80,
                id: 'linkurl',
                name: 'linkurl'
            },
            {
                xtype: 'filefield',
                name: 'product_forbid_banner',
                id: 'product_forbid_banner',
                fieldLabel: "商品失效圖片",
                msgTarget: 'side',
                buttonText: '選擇...',
                fileUpload: true,
                validator:
                function (value) {
                    if (value != '') {
                        var type = value.split('.');
                        var extention = type[type.length - 1].toString().toLowerCase();
                        if (extention == 'gif' || extention == 'png' || extention == 'jpg') {
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
                submitValue: true,
                labelWidth: 80
            },
            {
                xtype: 'filefield',
                name: 'product_active_banner',
                id: 'product_active_banner',
                fieldLabel: "商品有效圖片",
                msgTarget: 'side',
                buttonText: '選擇...',
                fileUpload: true,
                validator:
                function (value) {
                    if (value != '') {
                        var type = value.split('.');
                        var extention = type[type.length - 1].toString().toLowerCase();
                        if (extention == 'gif' || extention == 'png' || extention == 'jpg') {
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
                submitValue: true,
                labelWidth: 80
            }
        ],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        var oldStatus = 0; //修改時原數據的狀態為不啟用，要修改為啟用時，並且當前啟用值大於等於限制值，並且值存在時才提示
                        if (Row) {
                            oldStatus = Row.data.vote_status;
                        }
                        form.submit({
                            params: {
                                id: Ext.getCmp("map_id").getValue(),
                                bag_id: Ext.getCmp("bag_id").getValue(),
                                map_sort: Ext.getCmp("map_sort").getValue(),
                                product_id: Ext.getCmp("product_id").getValue(),
                                linkurl: Ext.getCmp("linkurl").getValue(),
                                product_forbid_banner: Ext.htmlEncode(Ext.getCmp('product_forbid_banner').getValue()),
                                product_active_banner: Ext.htmlEncode(Ext.getCmp('product_active_banner').getValue()),
                                ad_product_id: Ext.getCmp("ad_product_id").getValue(),
                                product_desc: Ext.getCmp("product_desc").getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert("提示信息", result.msg);
                                    editWin.close();
                                    ChinatrustMapBag.load();
                                } else {
                                    Ext.Msg.alert("提示信息", result.msg);
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        })
                    }
                }
            }
        ]
    });
    var editWin = Ext.create('Ext.window.Window', {
        title: "中信商品",
        id: 'editWin',
        iconCls: Row ? "icon-user-edit" : "icon-user-add",
        width: 500,
        layout: 'fit',
        items: [editFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: "關閉窗口",
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm("確認", "是否確定關閉窗口?", function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (Row) {
                    Ext.getCmp("map_sort").show(true);
                    editFrm.getForm().loadRecord(Row); //如果是編輯的話
                    initForm(Row);
                } else {
                    editFrm.getForm().reset(); //如果是編輯的話
                }
            }
        }
    });
    editWin.show();
    function initForm(Row) {
        var img = Row.data.product_forbid_banner;
        var imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        Ext.getCmp('product_forbid_banner').setRawValue(imgUrl);
        img = Row.data.product_active_banner;
        imgUrl = img.substring(img.lastIndexOf("\/") + 1);
        Ext.getCmp('product_active_banner').setRawValue(imgUrl);
        Ext.getCmp('map_sort').hidden(false);
    }
}