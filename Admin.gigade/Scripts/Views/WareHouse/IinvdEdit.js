function addFunction(row, store) {
    var cde_dt_shp;
    var pwy_dte_ctl;
    var cde_dt_var;
    var cde_dt_incr;
    var vendor_id;
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield', 
        layout: 'anchor',
        labelWidth: 120,
        url: '/WareHouse/InsertIinvd',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'textfield',
                fieldLabel: "商品品號/條碼",
                name: 'plas_prod_id',
                id: 'plas_prod_id',
                //labelWidth: 120,
                allowBlank:false,
                listeners: {
                    blur: function () {
                        var id = Ext.getCmp('plas_prod_id').getValue();
                        Ext.Ajax.request({
                            url: "/WareHouse/Getprodbyid",
                            method: 'post',
                            type: 'text',
                            params: {
                                id: Ext.getCmp('plas_prod_id').getValue()
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.success) {                                   
                                    msg = result.msg;
                                    locid = result.locid;
                                    vendor_id = result.vendor_id;
                                    P_itemid = result.item_id;
                                    Ext.getCmp("product_name").setValue(msg);                                    
                                    if (result.day == "")
                                    {
                                        Ext.getCmp("cde_dt_var").setValue("999");
                                    }
                                    else
                                    {
                                        Ext.getCmp("cde_dt_var").setValue(result.day);
                                    }                                
                                    if (locid.length > 0) {
                                        Ext.getCmp("loc_id").setValue(locid);
                                    } else {
                                        Ext.getCmp("loc_id").setValue("無");
                                    }
                                    Ext.getCmp("Pitem_id").setValue(P_itemid);
                                    cde_dt_shp = result.cde_dt_shp;
                                    pwy_dte_ctl = result.pwy_dte_ctl;
                                    cde_dt = result.cde_dt;
                                    cde_dt_var = result.cde_dt_var;
                                    cde_dt_incr = result.cde_dt_incr;
                                    //設置為可用
                                    //Ext.getCmp('startTime').setDisabled(false);
                                    Ext.getCmp('prod_qty').show();
                                    Ext.getCmp('plas_loc_id').setDisabled(false);
                                    Ext.getCmp('plas_loc_id').allowBlank = false;
                                    if (pwy_dte_ctl != "Y") {
                                        //如果不是有效期控管的商品就不顯示填寫時間
                                        Ext.getCmp("createtime").hide();
                                        Ext.getCmp("cdttime").hide();
                                        Ext.getCmp('cde_dt').setDisabled(false);
                                        Ext.getCmp('startTime').allowBlank = true;
                                        Ext.getCmp('cde_dt').allowBlank = true;
                                    }
                                    else {
                                        Ext.getCmp('createtime').show();
                                        Ext.getCmp('cdttime').show();
                                        Ext.getCmp('us1').setDisabled(false);
                                        Ext.getCmp('us2').setDisabled(false);
                                        Ext.getCmp('cde_dt').setDisabled(true);
                                        Ext.getCmp('startTime').setDisabled(false);
                                        Ext.getCmp('startTime').allowBlank = false;
                                        Ext.getCmp('cde_dt').allowBlank = false;
                                    }
                                }
                                else {
                                    Ext.getCmp("product_name").setValue("沒有該商品信息！");
                                    Ext.getCmp('startTime').setDisabled(true);
                                    Ext.getCmp('plas_loc_id').setDisabled(true);
                                    Ext.getCmp('us1').setDisabled(true);
                                    Ext.getCmp('us2').setDisabled(true);
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
                id: 'product_name',
                allowBlank: false
            },
             {
                 xtype: 'displayfield',
                 fieldLabel: "細項編號",
                 name: 'Pitem_id',
                 id: 'Pitem_id',
                 hidden: true
            
             },
            {
                xtype: 'numberfield',
                fieldLabel: "數量",
                name: 'prod_qty',
                id: 'prod_qty',
                hidden:true,
                minValue: 1,
                allowBlank:false
            },
            {
                xtype: 'fieldcontainer',
                fieldLabel: "製造日期",
                combineErrors: true,
                height: 24,
                margins: '0 200 0 0',
                layout: 'hbox',
                id: 'createtime',
                hidden: true,
                defaults: {
                    flex: 1,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'us',
                        inputValue: "1",
                        id: "us1",
                        checked: true,
                        disabled: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var i = Ext.getCmp('startTime');//製造日期
                                var j = Ext.getCmp('cde_dt');
                                if (newValue)
                                {
                                    j.allowBlank = true;
                                    i.setDisabled(false);
                                    j.setDisabled(true);
                                    j.setValue(null);
                                    i.allowBlank = false;
                                }
                            }
                        }
                    },
                    {
                        xtype: "datefield",
                        fieldLabel: "製造日期",
                        id: 'startTime',
                        name: 'startTime',
                        allowBlank: false,
                        submitValue: true,
                        listeners: {                            
                            change: function (radio, newValue, oldValue) {
                                if (Ext.getCmp('startTime').getValue() != "" && Ext.getCmp('startTime').getValue() != null) {
                                    Ext.Ajax.request({
                                        url: "/WareHouse/JudgeDate",
                                        method: 'post',
                                        type: 'text',
                                        params: {
                                            dtstring: 1,
                                            item_id: Ext.getCmp('plas_prod_id').getValue(),
                                            startTime: Ext.Date.format(Ext.getCmp('startTime').getValue(), 'Y-m-d')
                                        },
                                        success: function (form, action) {
                                            var result = Ext.decode(form.responseText);
                                            if (result.success) {
                                                if (result.msg == "1") {
                                                    Ext.Msg.alert(INFORMATION, "該商品製造日期大於今天");
                                                    Ext.getCmp('startTime').setValue(null);
                                                } else if (result.msg == "2") {
                                                    editFunction("超過允收天數");
                                                    return;
                                                } else if (result.msg == "3") {
                                                    editFunction("超過允出天數!");
                                                    return;
                                                } else if (result.msg == "4") {
                                                    editFunction("該商品已超過有效日期!");
                                                    return;
                                                }
                                            }
                                        }
                                    });
                                }
                            }
                        }                        
                    }
                ]
            },              
            {
                xtype: 'fieldcontainer',
                fieldLabel: "有效日期",
                combineErrors: true,
                hidden: true,
                layout: 'hbox',
                height:24,
                //margins: '0 200 0 0',
                id: 'cdttime',
                defaults: {
                    flex: 1,
                    hideLabel: true
                },
                items: [
                    {
                        xtype: 'radiofield',
                        name: 'us',
                        inputValue: "2",
                        id: "us2",
                        disabled: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                var i = Ext.getCmp('startTime');//製造日期
                                var j = Ext.getCmp('cde_dt');
                            
                                if (newValue) {
                                    i.setDisabled(true);
                                    j.setDisabled(false);
                                    i.allowBlank = true;
                                    i.setValue(null);
                                    j.allowBlank = false;
                                }
                            }
                        }
                    },
                    {
                        xtype: "datefield",
                        fieldLabel: "有效日期",
                        id: 'cde_dt',
                        name: 'cde_dt',
                        allowBlank: true,
                        submitValue: true,
                        listeners: {
                            change: function (radio, newValue, oldValue) {
                                if (Ext.getCmp('cde_dt').getValue() != "" && Ext.getCmp('cde_dt').getValue() != null)
                                    {
                                Ext.Ajax.request({
                                    url: "/WareHouse/JudgeDate",
                                    method: 'post',
                                    type: 'text',
                                    params: {
                                        dtstring: 2,
                                        item_id: Ext.getCmp('plas_prod_id').getValue(),
                                        startTime: Ext.Date.format(Ext.getCmp('cde_dt').getValue(), 'Y-m-d')
                                    },
                                    success: function (form, action) {
                                        var result = Ext.decode(form.responseText);
                                        if (result.success) {
                                            if (result.msg == "1") {
                                                Ext.Msg.alert(INFORMATION, "該商品製造日期大於今天");
                                                Ext.getCmp('startTime').setValue(null);
                                            } else if (result.msg == "2") {
                                                editFunction("超過允收天數");
                                                return;
                                            } else if (result.msg == "3") {
                                                editFunction("超過允出天數!");
                                                return;
                                            } else if (result.msg == "4") {
                                                editFunction("該商品已超過有效日期!");
                                                return;
                                            }
                                        }
                                    },
                                    failure: function (form, action) {
                                        Ext.Msg.alert(INFORMATION, "保存失敗,如有重複錯誤信息,請記錄詳細信息告知管理員");
                                    }
                                });
                                }
                            }
                        }
                    }
                ]
            },
            {
                xtype: 'textfield',
                fieldLabel: "上架料位",
                id: 'plas_loc_id',
                name: 'plas_loc_id',
                regex: /^[A-Za-z]{2}\d{3}[A-Za-z]\d{2}$/,
                regexText: "料位不規則",
                allowBlank: false,
                disabled:true
            },
            {
                xtype: 'displayfield',
                fieldLabel: "主料位",
                id: 'loc_id',
                name: 'loc_id'
            },
            {
                xtype: 'displayfield',
                fieldLabel: "允收天數",
                id: 'cde_dt_var',
                name: 'cde_dt_var',
                allowBlank:false
            }
        ],
        buttons: [{
            text: SAVE,
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();
                var isloc = 0;               
                Ext.Ajax.request({
                    url: "/WareHouse/Islocid",
                    method: 'post',
                    type: 'text',
                    async: false,//是否異步
                    params: {
                        plas_loc_id: Ext.getCmp('plas_loc_id').getValue(),//料位
                        prod_id: Ext.getCmp('Pitem_id').getValue(),//商品品號
                        loc_id: Ext.getCmp('loc_id').getValue()//主料位

                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        isloc = result.msg;
                    }
                });
                if (isloc == 6) {//表示驗證成功
                    if (form.isValid()) {
                        var myMask = new Ext.LoadMask(Ext.getBody(), { msg: 'Loading...' });
                        myMask.show();
                        Ext.Ajax.request({
                            url: "/WareHouse/GetSearchStock",
                            params: {
                                loc_id: Ext.getCmp("plas_loc_id").getValue(),
                                item_id: Ext.getCmp("Pitem_id").getValue(),
                                cde_date: Ext.getCmp('cde_dt').getValue(),
                                made_date: Ext.getCmp('startTime').getValue()
                            },
                            success: function (response) {
                               
                                var result = Ext.decode(response.responseText);
                                
                                if (result.msg != "0") {
                                    myMask.hide();
                                    Ext.Msg.alert("提示", "該料位有上鎖的庫存，不能庫調!");
                                }
                                else {
                                  
                                   
                                    form.submit({
                                        params: {
                                            //id: Ext.htmlEncode(Ext.getCmp('plas_prdd_id').getValue()),//條碼
                                            iialg: 'N',//寄倉流程 新增庫存用到
                                            item_id: Ext.htmlEncode(Ext.getCmp('plas_prod_id').getValue()),//商品品號
                                            product_name: Ext.htmlEncode(Ext.getCmp('product_name').getValue()),//品名
                                            prod_qty: Ext.htmlEncode(Ext.getCmp('prod_qty').getValue()),//數量
                                            startTime: Ext.htmlEncode(Ext.getCmp('startTime').getValue()),//創建時間
                                            cde_dt: Ext.htmlEncode(Ext.getCmp('cde_dt').getValue()),//有效時間
                                            plas_loc_id: Ext.htmlEncode(Ext.getCmp('plas_loc_id').getValue()),//上架料位
                                            loc_id: Ext.getCmp('loc_id').getValue(),//主料位
                                            cde_dt_var: Ext.htmlEncode(Ext.getCmp('cde_dt_var').getValue()),
                                            cde_dt_incr: cde_dt_incr,
                                            iarc_id: '',//不存值
                                            doc_num: '',//庫調單號
                                            Po_num: '',//前置單號
                                            remark: '',//備註
                                            vendor_id: vendor_id
                                        },
                                        success: function (form, action) {
                                            var result = Ext.decode(action.response.responseText);
                                            Ext.Msg.alert(INFORMATION, SUCCESS);
                                            if (result.success) {
                                                myMask.hide();
                                                IinvdStore.load();
                                                editWin.close();
                                            } else {
                                                myMask.hide();
                                                Ext.MessageBox.alert(ERRORSHOW + result.success);
                                            }
                                        },
                                        failure: function () {
                                            myMask.hide();
                                            Ext.Msg.alert(INFORMATION, "提交失敗!");
                                        }
                                    });
                                }
                            }
                        })
                    }
                }
                else if (isloc == 1) {
                    Ext.MessageBox.alert(INFORMATION, "上架料位不存在或已占用！");
                } else if (isloc == 2) {
                    Ext.MessageBox.alert(INFORMATION,"上架料位被其他商品佔用！");
                } else if (isloc == 3) {
                    Ext.MessageBox.alert(INFORMATION,"上架料位已鎖定！");
                }
            }
        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "收貨上架",
        id: 'editWin',
        iconCls: 'icon-user-edit',
        width: 550,
        height: 380,
        autoScroll:true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [editFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
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
         }]
        ,
        listeners: {
            'show': function () {
                if (row == null) {
                    // editFrm.getForm().loadRecord(row); //如果是添加的話
                    editFrm.getForm().reset();
                } else {
                    editFrm.getForm().loadRecord(row); //如果是編輯的

                }
            }
        }
    });
    editWin.show();
}
