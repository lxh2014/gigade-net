//調度流程
Ext.onReady(function () {
    var cde_dt_shp;
    var pwy_dte_ctl = "N";
    var cde_date = new Date;
    var AssgInformation = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 1000,
        height: 150,
        border: false,
        plain: true,
        bodyPadding: 20,
        id: 'aform',
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '工作代號',
                id: 'assg_id',
                name: 'assg_id',
                labelWidth: 120,
                submitValue: true
            },
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                width: 1000,
                layout: 'hbox',
                items: [
                    {
                        xtype: 'displayfield',
                        fieldLabel: '訂單編號',
                        id: 'ord_id',
                        name: 'ord_id',
                        labelWidth: 120,
                        value: document.getElementById("ord_id").value,
                        submitValue: true
                    },
                    {
                        xtype: 'displayfield',
                        fieldLabel: '備註',
                        margin: '0 0 0 50',
                        id: 'note_order',
                        name: 'note_order',
                        hidden: true
                    }
                ]
            },
            {
                xtype: 'displayfield',
                fieldLabel: '出貨單號',
                id: 'deliver_code',
                name: 'deliver_code',
                labelWidth: 120,
                submitValue: true
            },
            {
                xtype: 'textfield',
                id: 'item_id',
                fieldLabel: PUPCID,
                labelWidth: 120,
                submitValue: true,
                allowBlank: false,
                listeners: {
                    change: function () {
                        if (Ext.getCmp('item_id').getValue().length > 5)
                        {
                            ProductInformation.form.load({
                                url: '/WareHouse/GetMarkTallyTW',
                                method: 'GET',
                                params: {
                                    type: 1,
                                    item_id: Ext.getCmp('item_id').getValue(),
                                    ord_id: Ext.getCmp('ord_id').getValue(),
                                    freight_set: Ext.getCmp('assg_id').getValue(),
                                    deliver_code: Ext.getCmp('deliver_code').getValue()
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    var values = Ext.Object.getValues(result.data);
                                    if (values.length !== 0) {
                                        if (result.data.item_id != null) {
                                            Ext.getCmp("err").show();
                                            Ext.getCmp("err").setValue("<div style='color:red;'>這個條碼對應到" + result.data.item_id + "品號，請重新掃描正確商品!</div>");
                                            Ext.getCmp('item_id').setValue('');
                                            Ext.getCmp('item_id').focus();
                                        }
                                        else {
                                            Ext.getCmp("err").hide();
                                            cde_dt_shp = result.data.cde_dt_shp;
                                            pwy_dte_ctl = result.data.pwy_dte_ctl;
                                            Ext.getCmp('pform').show();
                                            if (pwy_dte_ctl == "Y") {
                                                Ext.getCmp('cde_dt').allowBlank = false;
                                                Ext.getCmp('cde_dt').show();
                                                Ext.getCmp('cde_dt').setValue('');
                                                Ext.getCmp('made_dt').allowBlank = false;
                                                Ext.getCmp('made_dt').show();
                                                Ext.getCmp('made_dt').setValue(new Date);
                                            }
                                            Ext.getCmp('act_pick_qty').focus();

                                            if (result.data.note_order.length > 0) {
                                                Ext.getCmp("note_order").setValue(result.data.note_order);
                                                Ext.getCmp("note_order").show();
                                            }
                                            else {
                                                Ext.getCmp("note_order").hide();
                                            }
                                        }
                                    }
                                    else {
                                        Ext.getCmp("err").show();
                                        Ext.getCmp("err").setValue("<div style='color:red;'>該商品已撿完或不存在，請重新輸入!</div>");
                                        Ext.getCmp('item_id').setValue('');
                                        Ext.getCmp('item_id').focus();
                                    }
                                },
                                failure: function (form, action) {
                                    Ext.Msg.alert(INFORMATION, "数据加载失敗! ");
                                }
                            });
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                fieldLabel: '提示',
                id: 'err',
                name: 'err',
                style: 'color:red;',
                width: 600,
                hidden: true
            }
        ],
        buttonAlign: 'center'
    });
    var ProductInformation = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        url: "/WareHouse/GETMarkTallyWD",
        width: 1000,
        height: 240,
        border: false,
        plain: true,
        bodyPadding: 20,
        id: 'pform',
        hidden: true,
        items: [
            {
                xtype: 'displayfield',
                fieldLabel: '商品名稱',
                labelWidth: 120,
                anchor: "95%",
                id: 'product_name',
                name: 'product_name',
                readOnly: true,
                submitValue: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '訂單細項',
                id: 'ordd_id',
                name: 'ordd_id ',
                labelWidth: 120,
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '流水号',
                id: 'seld_id',
                name: 'seld_id ',
                labelWidth: 120,
                hidden: true,
                submitValue: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '訂貨量',
                id: 'ord_qty',
                name: 'ord_qty ',
                labelWidth: 120,
                submitValue: true,
                hidden: true
            },
            {
                xtype: 'displayfield',
                fieldLabel: '訂貨數量',
                id: 'out_qty',
                name: 'out_qty ',
                labelWidth: 120,
                submitValue: true
            },
            {
                xtype: 'numberfield',
                id: 'act_pick_qty',
                fieldLabel: '實際撿貨量',
                labelWidth: 120,
                value: 0,
                minValue: 0,
                submitValue: true,
                allowBlank: false,
                listeners: {
                    afterrender: function () {
                        var out_qty = Ext.getCmp("out_qty").getValue();
                        Ext.getCmp("act_pick_qty").setMaxValue(parseInt(out_qty));
                        Ext.getCmp('delivercode').focus();
                    }
                }
            },
            {
                xtype: 'datefield',
                format: 'Y-m-d',
                id: 'cde_dt',
                fieldLabel: '有效日期',
                labelWidth: 120,
                editable: false,
                hidden: true,
                listeners: {
                    change: function () {
                        //if (Ext.getCmp('cde_dt').getValue() !=null) {
                        //    //if (Ext.Date.format(Ext.getCmp('cde_dt').getValue(), 'Y-m-d') != cde_date) {
                        //    //    //Ext.getCmp("made_dt").hide();
                        //    //    //Ext.getCmp("made_dt").setValue('');
                        //    //    Ext.getCmp('made_dt').allowBlank = true;
                        //    //}
                        //}
                        if (Ext.getCmp('cde_dt').getValue() != "" && Ext.getCmp('cde_dt').getValue() != null) {
                            Ext.Ajax.request({
                                url: "/WareHouse/JudgeDate",
                                method: 'post',
                                type: 'text',
                                params: {
                                    dtstring: 2,
                                    item_id: Ext.getCmp('item_id').getValue(),
                                    startTime: Ext.Date.format(Ext.getCmp('cde_dt').getValue(), 'Y-m-d')
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        if (result.msg == "1") {
                                            Ext.Msg.alert(INFORMATION, "該商品製造日期大於今天");
                                            Ext.getCmp('cde_dt').setValue(null);
                                            Ext.getCmp('made_dt').setValue(null);
                                        } else if (result.msg == "3") {
                                            Ext.getCmp('made_dt').setValue(null);
                                            editFunction("超過允出天數!");
                                            return;
                                        } else if (result.msg == "4") {
                                            Ext.getCmp('made_dt').setValue(null);
                                            editFunction("有效期為" + result.dte + "的商品已超過有效日期!");
                                            return;
                                        } else if (result.msg == "5") {
                                            Ext.getCmp('made_dt').setValue(result.dts);
                                        }
                                    }
                                },
                                failure: function (form, action) {
                                }
                            });
                            Ext.getCmp('delivercode').focus();
                        }
                    }
                }
            },
            {
                xtype: 'datefield',
                format: 'Y-m-d',
                id: 'made_dt',
                fieldLabel: '製造日期',
                labelWidth: 120,
                editable: false,
                hidden: true,
                listeners: {
                    change: function () {
                        //if (Ext.getCmp('cde_dt').getValue() != null) {
                        //    if (Ext.Date.format(Ext.getCmp('made_dt').getValue(), 'Y-m-d') != Ext.Date.format(new Date, 'Y-m-d')) {
                        //        //Ext.getCmp("cde_dt").hide();
                        //        //Ext.getCmp("cde_dt").setValue('');
                        //        Ext.getCmp('cde_dt').allowBlank = true;
                        //    }
                        //}
                        if (Ext.getCmp('made_dt').getValue() != "" && Ext.getCmp('made_dt').getValue() != null) {
                            Ext.Ajax.request({
                                url: "/WareHouse/JudgeDate",
                                method: 'post',
                                type: 'text',
                                params: {
                                    dtstring: 1,
                                    item_id: Ext.getCmp('item_id').getValue(),
                                    startTime: Ext.Date.format(Ext.getCmp('made_dt').getValue(), 'Y-m-d')
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        if (result.msg == "1") {
                                            Ext.Msg.alert(INFORMATION, "該商品製造日期大於今天");
                                            Ext.getCmp('cde_dt').setValue(null);
                                            Ext.getCmp('made_dt').setValue(null);
                                        } else if (result.msg == "3") {
                                            Ext.getCmp('cde_dt').setValue(null);
                                            editFunction("超過允出天數!");
                                            return;
                                        } else if (result.msg == "4") {
                                            Ext.getCmp('cde_dt').setValue(null);
                                            editFunction("有效期為" + result.dte + "的商品已超過有效日期!");
                                            return;
                                        } else if (result.msg == "5") {
                                            cde_date = result.dte;
                                            Ext.getCmp('cde_dt').setValue(cde_date);
                                        }
                                    }
                                }
                            });
                        }
                        Ext.getCmp('delivercode').focus();
                    }
                }
            },
            {
                xtype: 'textfield',
                id: 'delivercode',
                fieldLabel: '出貨單號',
                labelWidth: 120,
                submitValue: true,
                allowBlank: false,
                listeners: {
                    change: function () {
                        var delivercode = Ext.getCmp("delivercode").getValue().toUpperCase();
                        var deliver_code = Ext.getCmp("deliver_code").getValue();
                        if (delivercode.length > 0) {
                            if (delivercode.length == "9") {
                                if (delivercode == deliver_code) {
                                    Ext.getCmp("err1").hide();
                                    Ext.getCmp("btn_sure").setDisabled(false);
                                }
                                else {
                                    Ext.getCmp("err1").show();
                                    Ext.getCmp("delivercode").setValue("");
                                    Ext.getCmp("delivercode").focus();
                                    Ext.getCmp("btn_sure").setDisabled(true);
                                }
                            }
                        } else {
                            Ext.getCmp("btn_sure").setDisabled(true);
                        }

                    }
                }
            }, 
        {
            id: 'err1',
            border: false,
            html: '<div style="color:red;">提示：不是該出貨單的單號,請重新輸入!</div>',
            hidden: true
        }],
        
        buttonAlign: 'center',
        buttons: [{
            formBind: true,
            disabled: true,
            text: '確定',
            id: 'btn_sure',
            handler: function () {
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            assg_id: Ext.htmlEncode(Ext.getCmp('assg_id').getValue()),
                            ord_id: Ext.htmlEncode(Ext.getCmp('ord_id').getValue()),
                            item_id: Ext.htmlEncode(Ext.getCmp('item_id').getValue()),
                            out_qty: Ext.htmlEncode(Ext.getCmp('out_qty').getValue()),
                            ord_qty: Ext.htmlEncode(Ext.getCmp('ord_qty').getValue()),
                            act_pick_qty: Ext.htmlEncode(Ext.getCmp('act_pick_qty').getValue()),
                            ordd_id: Ext.htmlEncode(Ext.getCmp('ordd_id').getValue()),
                            seld_id: Ext.htmlEncode(Ext.getCmp('seld_id').getValue()),
                            cde_dt: Ext.getCmp("cde_dt").getValue(),
                            made_dt: Ext.getCmp("made_dt").getValue(),
                            commodity_type: 3,
                            deliver_id: Ext.htmlEncode(Ext.getCmp('delivercode').getValue())
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == 1)//商品未檢完，跳到輸入商品編號/條碼頁面
                                {
                                    Ext.Msg.alert(INFORMATION, "該訂單尙有商品未撿！", function () {
                                        document.location.href = "/WareHouse/MarkTallyTW?number=" + document.getElementById("ord_id").value + "&freight_set=" + document.getElementById("freight_set").value;//繼續輸入產品條碼
                                   
                                    });//跳轉輸入訂單號頁面 
                                }
                                else if (result.msg == 2)//商品檢完未檢夠有缺貨-跳到輸入訂單號頁面
                                {
                                    Ext.Msg.alert(INFORMATION, "該訂單下商品已撿完,有缺貨！", function () {
                                        document.location.href = "/WareHouse/MarkTally";
                                    });//跳轉輸入訂單號頁面

                                }
                                else if (result.msg == 3)//此訂單下的寄倉+調度都檢夠
                                {
                                    Ext.Msg.alert(INFORMATION, "該訂單可以封箱！", function () {
                                        document.location.href = "/WareHouse/MarkTally";
                                    });

                                }
                                else if (result.msg == 4)//此訂單下的寄倉尚未檢夠
                                {
                                    Ext.Msg.alert(INFORMATION, "該訂單下調度商品已撿完,但有寄倉商品未檢完！", function () {
                                        document.location.href = "/WareHouse/MarkTally";
                                    });//跳轉輸入訂單號頁面

                                }
                                else if (result.msg == 5)//此訂單下的寄倉尚未檢夠
                                {
                                    editFunction("請移交主管處理");
                                }
                            }
                            else {
                                Ext.Msg.alert(INFORMATION, result.Msg);
                            }
                        },
                        failure: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.msg != undefined) {
                                Ext.Msg.alert(INFORMATION, result.msg);
                            }
                        }
                    });
                }
            }

        }]
    });

    var MarkTWForm = Ext.create('Ext.form.Panel', {
        layout: 'vbox',
        border: false,
        width: 1000,
        height: 450,
        id: 'ffrom',
        items: [AssgInformation, ProductInformation]
    });
    AssgInformation.form.load({
        url: '/WareHouse/GetMarkTallyTW',
        method: 'GET',
        params: {
            type: 0,
            ord_id: document.getElementById("ord_id").value,
            freight_set: document.getElementById("freight_set").value
        },
        success: function (form, action) {
            var result = Ext.decode(action.response.responseText);
            var values = Ext.Object.getValues(result.data);
            if (values.length == 0) {
                Ext.Msg.alert(INFORMATION, "此訂單下沒有調度商品！ ");
                window.location.href = '/WareHouse/MarkTally';
            }
        },
        failure: function (form, action) {
            Ext.Msg.alert(INFORMATION, "数据加载失敗! ");
        }
    });
    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        height: 1600,
        items: [MarkTWForm],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                //gdIupc.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    Ext.getCmp('item_id').focus();
})
