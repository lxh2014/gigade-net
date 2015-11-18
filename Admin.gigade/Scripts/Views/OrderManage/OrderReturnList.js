Ext.onReady(function () {

    Ext.define('gigade.zipAddress', {
        extend: 'Ext.data.Model',
        fields: [
            { name: "zipcode", type: "string" },
            { name: "zipname", type: "string" }
        ]
    });
    var zipStore = Ext.create('Ext.data.Store', {
        model: 'gigade.zipAddress',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/VipUserGroup/GetZipStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.typeStore', {
        extend: 'Ext.data.Model',
        fields: [
               { name: "parameterCode", type: "int" },
            { name: "parameterName", type: "string" }
        ],
    });

    var typeStore = Ext.create('Ext.data.Store', {
        model: 'gigade.typeStore',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/OrderManage/GetOrcTypeStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.InvoiceDeal', {
        extend: 'Ext.data.Model',
        fields: [
               { name: "parameterCode", type: "int" },
            { name: "parameterName", type: "string" }
        ],
    });

    var  InvoiceDealStore = Ext.create('Ext.data.Store', {
        model: 'gigade.InvoiceDeal',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/OrderManage/GetInvoiceDealStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    Ext.define('gigade.package', {
        extend: 'Ext.data.Model',
        fields: [
               { name: "parameterCode", type: "int" },
            { name: "parameterName", type: "string" }
        ],
    });

    var PackageStore = Ext.create('Ext.data.Store', {
        model: 'gigade.package',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: "/OrderManage/GetPackageStore",
            reader: {
                type: 'json',
                root: 'data'
            }
        }
    });

    var DateStore = Ext.create('Ext.data.Store', {
        fields: ['txt', 'value'],
        data: [
            { "txt": "上午", "value": "0" },
            { "txt": "下午", "value": "1" },
            { "txt": "晚上", "value": "2" },
            { "txt": "隨時", "value": "3" }
        ]
    });

    Ext.define('gigade.CouldReturn', {
        extend: 'Ext.data.Model',
        fields: [
        { name: "order_id", type: "int" },
        { name: "item_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "product_spec_name", type: "string" },
        { name: "buy_num", type: "int" },
        { name: "single_money", type: "int" },
        { name: "product_mode", type: "string" }
        ]
    });

    var CouldReturnStore = Ext.create('Ext.data.Store', {
        model: 'gigade.CouldReturn',
        autoLoad: false,
        proxy: {
            type: 'ajax',
            url: '/OrderManage/CouldGridList',
            reader: {
                type: 'json',
                root: 'data',
                totalProperty: 'totalCount'
            }
        }
    });
    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
            }
        }
    });

    //輸入退款編號然後確定顯示什麼
    var orderReturnForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/OrderManage/CheckOrderId',
        margin: '0 10 0 0',
        border: false,
        plain: true,
        bodyPadding:'15',
        id: 'orderReturnForm',
        items: [
        {
            xtype: 'textfield',
            fieldLabel: '請輸入退款單號',
            id: 'return_id',
            name: 'return_id',
            labelWidth: 125,
            allowBlank: false,
            listeners: {
                specialkey: function (field, e) {
                    if (e.getKey() == e.ENTER) {
                     
                        var regex = /^[0-9]*$/;
                        if (regex.test(Ext.getCmp('return_id').getValue())) {
                            var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                            myMask.show();
                            Ext.Ajax.request({
                                url: '/OrderManage/CheckOrderId',
                                params: {
                                    return_id: Ext.getCmp('return_id').getValue(),
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    myMask.hide();
                                    if (result.success) {
                                        myMask.hide();
                                        if (result.status == -1) {
                                            orderDeliverForm.hide();
                                            transport.hide();
                                            transport.getForm().reset();
                                            Ext.Msg.alert("提示信息", "無此退款單號或已歸檔！");
                                            orderReturnForm.getForm().reset();
                                            orderDeliverForm.getForm().reset();
                                            CouldReturnGrid.hide();
                                            transportBtn
                                            Ext.getCmp('transportBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp('wareHouseBtn').setDisabled(true);
                                            Ext.getCmp('returnMoney').setDisabled(true);
                                            Ext.getCmp("bank_name").hide();
                                            Ext.getCmp("bank_branch").hide();
                                            Ext.getCmp("bank_account").hide();
                                            Ext.getCmp("account_name").hide();
                                            Ext.getCmp("bank_note").hide();
                                            
                                        }
                                        else if (result.status == 0) {
                                            if (result.isZIChu == 1) {
                                                Ext.getCmp('isZIChu').setValue(result.isZIChu);
                                            }
                                            transport.getForm().reset();
                                            Ext.getCmp('transportBtn').setDisabled(true);
                                            Ext.getCmp('wareHouseBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(true);
                                            Ext.getCmp('returnMoney').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp('orc_service_remark').setValue("");
                                            Ext.getCmp('orc_remark').setValue("");
                                            Ext.getCmp('orc_type').setValue(1);
                                            Ext.getCmp('invoice_deal').setValue(1);
                                            Ext.getCmp('orc_name').setValue(result.name);
                                            Ext.getCmp('orc_phone').setValue(result.mobile);
                                            Ext.getCmp('orc_zipcode').setValue(result.zipcode);
                                            Ext.getCmp('orc_address').setValue(result.address);
                                            orderDeliverForm.show();
                                            transport.hide();
                                            CouldReturnGrid.hide();
                                            Ext.getCmp("bank_name").hide();
                                            Ext.getCmp("bank_branch").hide();
                                            Ext.getCmp("bank_account").hide();
                                            Ext.getCmp("account_name").hide();
                                            Ext.getCmp("bank_note").hide();
                                        }
                                        else if (result.status == 0.5) {
                                            orderDeliverForm.hide();
                                            CouldReturnGrid.hide();
                                            transport.show();
                                            Ext.getCmp('transportBtn').setDisabled(false);
                                            Ext.getCmp('wareHouseBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(true);
                                            Ext.getCmp('returnMoney').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp("bank_name").hide();
                                            Ext.getCmp("bank_branch").hide();
                                            Ext.getCmp("bank_account").hide();
                                            Ext.getCmp("account_name").hide();
                                            Ext.getCmp("bank_note").hide();
                                        }
                                        else if (result.status == 1) {
                                            orderDeliverForm.hide();
                                            CouldReturnGrid.show();
                                            transport.getForm().reset();
                                            Ext.getCmp('transportBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(false);
                                            Ext.getCmp('package').show();
                                            Ext.getCmp('wareHouseBtn').setDisabled(true);
                                            Ext.getCmp('returnMoney').setDisabled(true);
                                            Ext.getCmp("bank_name").hide();
                                            Ext.getCmp("bank_branch").hide();
                                            Ext.getCmp("bank_account").hide();
                                            Ext.getCmp("account_name").hide();
                                            Ext.getCmp("bank_note").hide();
                                            transport.hide();
                                            CouldReturnStore.load({ params: { return_id: Ext.getCmp('return_id').getValue()}});
                                        }
                                        else if (result.status == 2) {
                                            orderDeliverForm.hide();
                                            transport.getForm().reset();
                                            CouldReturnGrid.show();
                                            Ext.getCmp('transportBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp('wareHouseBtn').setDisabled(false);
                                            Ext.getCmp('returnMoney').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp("bank_name").hide();
                                            Ext.getCmp("bank_branch").hide();
                                            Ext.getCmp("bank_account").hide();
                                            Ext.getCmp("account_name").hide();
                                            Ext.getCmp("bank_note").hide();
                                            CouldReturnStore.load({ params: { return_id: Ext.getCmp('return_id').getValue() } });
                                            transport.hide();
                                        }
                                        else if (result.status == 3) {
                                            if (result.isZIChu == 1) {
                                                Ext.getCmp('isZIChu').setValue(result.isZIChu);
                                            }
                                            if (result.order_payment == 2) {
                                                Ext.getCmp("bank_name").show();
                                                Ext.getCmp("bank_branch").show();
                                                Ext.getCmp("bank_account").show();
                                                Ext.getCmp("account_name").show();
                                                Ext.getCmp("bank_note").show();
                                                Ext.getCmp("bank_name").setValue(result.bank_name);
                                                Ext.getCmp("bank_branch").setValue(result.bank_branch);
                                                Ext.getCmp("bank_account").setValue(result.bank_account);
                                                Ext.getCmp("account_name").setValue(result.account_name);
                                                Ext.getCmp("bank_note").setValue(result.bank_note);
                                            }
                                            transport.getForm().reset();
                                            Ext.getCmp('transportBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp('wareHouseBtn').setDisabled(true);
                                            Ext.getCmp('returnMoney').setDisabled(false);
                                            orderDeliverForm.hide();
                                            CouldReturnGrid.show();
                                            transport.hide();
                                            CouldReturnStore.load({ params: { return_id: Ext.getCmp('return_id').getValue() } });
                                            //需確認退款單
                                        }
                                        else if (result.status == 4) {
                                            transport.getForm().reset();
                                            Ext.getCmp('transportBtn').setDisabled(true);
                                            Ext.getCmp('couldReturnBtn').setDisabled(true);
                                            Ext.getCmp('package').hide();
                                            Ext.getCmp('wareHouseBtn').setDisabled(true);
                                            Ext.getCmp('returnMoney').setDisabled(true);
                                            Ext.getCmp("bank_name").hide();
                                            Ext.getCmp("bank_branch").hide();
                                            Ext.getCmp("bank_account").hide();
                                            Ext.getCmp("account_name").hide();
                                            Ext.getCmp("bank_note").hide();
                                            orderDeliverForm.hide();
                                            CouldReturnGrid.hide();
                                            transport.hide();
                                         
                                            Ext.Msg.alert("提示信息", "已確認退款");
                                        }
                                    }
                                },
                                failure: function (form, action) {
                                    myMask.hide();
                                    Ext.Msg.alert("提示信息", "出現異常！");
                                }
                            });
                        }
                        else {
                            Ext.Msg.alert("提示信息", "退款單號為數字");
                            orderReturnForm.getForm().reset();
                        }



                    }
                }
            }
        },
        ],
        buttons: [{
            text: '確認',
            formBind: true,
            disabled: true,
            handler: function () {
                var regex = /^[0-9]*$/;
                if (regex.test(Ext.getCmp('return_id').getValue())) {
                    var form = this.up('form').getForm();
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    myMask.show();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                return_id: Ext.getCmp('return_id').getValue(),
                            },
                            success: function (form, action) {
                                 myMask.hide();
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    myMask.hide();
                                    if (result.status == -1) {
                                        orderDeliverForm.hide();
                                        transport.hide();
                                        transport.getForm().reset();
                                        Ext.Msg.alert("提示信息", "無此退款單號或已歸檔！");
                                        orderReturnForm.getForm().reset();
                                        orderDeliverForm.getForm().reset();
                                        CouldReturnGrid.hide();
                                        transportBtn
                                        Ext.getCmp('transportBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp('wareHouseBtn').setDisabled(true);
                                        Ext.getCmp('returnMoney').setDisabled(true);
                                        Ext.getCmp("bank_name").hide();
                                        Ext.getCmp("bank_branch").hide();
                                        Ext.getCmp("bank_account").hide();
                                        Ext.getCmp("account_name").hide();
                                        Ext.getCmp("bank_note").hide();
                                    }
                                    else if (result.status == 0) {
                                        if (result.isZIChu == 1) {
                                            Ext.getCmp('isZIChu').setValue(result.isZIChu);
                                        }
                                        transport.getForm().reset();
                                        Ext.getCmp('transportBtn').setDisabled(true);
                                        Ext.getCmp('wareHouseBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(true);
                                        Ext.getCmp('returnMoney').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp('orc_service_remark').setValue("");
                                        Ext.getCmp('orc_remark').setValue("");
                                        Ext.getCmp('orc_type').setValue(1);
                                        Ext.getCmp('invoice_deal').setValue(1);
                                        Ext.getCmp('orc_name').setValue(result.name);
                                        Ext.getCmp('orc_phone').setValue(result.mobile);
                                        Ext.getCmp('orc_zipcode').setValue(result.zipcode);
                                        Ext.getCmp('orc_address').setValue(result.address);
                                        orderDeliverForm.show();
                                        transport.hide();
                                        CouldReturnGrid.hide();
                                        Ext.getCmp("bank_name").hide();
                                        Ext.getCmp("bank_branch").hide();
                                        Ext.getCmp("bank_account").hide();
                                        Ext.getCmp("account_name").hide();
                                        Ext.getCmp("bank_note").hide();
                                    }
                                    else if (result.status == 0.5) {
                                        orderDeliverForm.hide();
                                        CouldReturnGrid.hide();
                                        transport.show();
                                        Ext.getCmp('transportBtn').setDisabled(false);
                                        Ext.getCmp('wareHouseBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(true);
                                        Ext.getCmp('returnMoney').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp("bank_name").hide();
                                        Ext.getCmp("bank_branch").hide();
                                        Ext.getCmp("bank_account").hide();
                                        Ext.getCmp("account_name").hide();
                                        Ext.getCmp("bank_note").hide();
                                    }
                                    else if (result.status == 1) {
                                        orderDeliverForm.hide();
                                        CouldReturnGrid.show();
                                        transport.getForm().reset();
                                        Ext.getCmp('transportBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(false);
                                        Ext.getCmp('package').show();
                                        Ext.getCmp('wareHouseBtn').setDisabled(true);
                                        Ext.getCmp('returnMoney').setDisabled(true);
                                        Ext.getCmp("bank_name").hide();
                                        Ext.getCmp("bank_branch").hide();
                                        Ext.getCmp("bank_account").hide();
                                        Ext.getCmp("account_name").hide();
                                        Ext.getCmp("bank_note").hide();
                                        transport.hide();
                                        CouldReturnStore.load({ params: { return_id: Ext.getCmp('return_id').getValue() } });
                                    }
                                    else if (result.status == 2) {
                                        orderDeliverForm.hide();
                                        transport.getForm().reset();
                                        CouldReturnGrid.show();
                                        Ext.getCmp('transportBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp('wareHouseBtn').setDisabled(false);
                                        Ext.getCmp('returnMoney').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp("bank_name").hide();
                                        Ext.getCmp("bank_branch").hide();
                                        Ext.getCmp("bank_account").hide();
                                        Ext.getCmp("account_name").hide();
                                        Ext.getCmp("bank_note").hide();
                                        CouldReturnStore.load({ params: { return_id: Ext.getCmp('return_id').getValue() } });
                                        transport.hide();
                                    }
                                    else if (result.status == 3) {
                                        if (result.isZIChu == 1) {
                                            Ext.getCmp('isZIChu').setValue(result.isZIChu);
                                        }
                                        if (result.order_payment == 2) {
                                            Ext.getCmp("bank_name").show();
                                            Ext.getCmp("bank_branch").show();
                                            Ext.getCmp("bank_account").show();
                                            Ext.getCmp("account_name").show();
                                            Ext.getCmp("bank_note").show();
                                            Ext.getCmp("bank_name").setValue(result.bank_name);
                                            Ext.getCmp("bank_branch").setValue(result.bank_branch);
                                            Ext.getCmp("bank_account").setValue(result.bank_account);
                                            Ext.getCmp("account_name").setValue(result.account_name);
                                            Ext.getCmp("bank_note").setValue(result.bank_note);
                                        }
                                        transport.getForm().reset();
                                        Ext.getCmp('transportBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp('wareHouseBtn').setDisabled(true);
                                        Ext.getCmp('returnMoney').setDisabled(false);
                                        orderDeliverForm.hide();
                                        CouldReturnGrid.show();
                                        transport.hide();
                                        CouldReturnStore.load({ params: { return_id: Ext.getCmp('return_id').getValue() } });
                                        //需確認退款單
                                    }
                                    else if (result.status == 4) {
                                        transport.getForm().reset();
                                        Ext.getCmp('transportBtn').setDisabled(true);
                                        Ext.getCmp('couldReturnBtn').setDisabled(true);
                                        Ext.getCmp('package').hide();
                                        Ext.getCmp('wareHouseBtn').setDisabled(true);
                                        Ext.getCmp('returnMoney').setDisabled(true);
                                        Ext.getCmp("bank_name").hide();
                                        Ext.getCmp("bank_branch").hide();
                                        Ext.getCmp("bank_account").hide();
                                        Ext.getCmp("account_name").hide();
                                        Ext.getCmp("bank_note").hide();
                                        
                                        orderDeliverForm.hide();
                                        CouldReturnGrid.hide();
                                        transport.hide();

                                        Ext.Msg.alert("提示信息", "已確認退款");
                                    }
                                }
                            },
                            failure: function (form, action) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息","出貨異常！");
                            }
                        });
                    }
                }
                else {
                    Ext.Msg.alert("提示信息", "退款款單號為數字");
                }
            }
        }],
    });
   //一大波的按鈕   還有當payment=2時，顯示銀行相關信息
    var buttonForm = Ext.create("Ext.form.Panel", {
        layout: 'anchor',
        width: 600,
        border: false,
        plain: true,
        bodyPadding:'15',
        id: 'buttonForm',
        items: [
           {
               xtype: 'fieldcontainer',
               items: [
                      //退貨類型
                       {
                           xtype: 'combobox',
                           fieldLabel: '退貨處理',
                           id: 'package',
                           name: 'package',
                           store: PackageStore,
                           displayField: 'parameterName',
                           valueField: 'parameterCode',
                           lastQuery: '',
                           value: '1',
                           hidden:true,
                           editable: false,
                       },
                       //這個是確認物流單的button
                       {
                           xtype: 'button',
                           text:'確認物流單',
                           id: 'transportBtn',
                           hidden: true,
                           disabled: true,
                           handler: function () {
                               var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                               myMask.show();
                               var return_id = Ext.getCmp('return_id').getValue();
                               var orc_deliver_code = Ext.getCmp('orc_deliver_code').getValue();
                               var orc_deliver_date = Ext.getCmp('orc_deliver_date').getValue();
                               var orc_deliver_time = Ext.getCmp('orc_deliver_time').getValue();
                               if (orc_deliver_code != "" && orc_deliver_date != null&&orc_deliver_code.length<=50) {
                                   Ext.Ajax.request({
                                       url: '/OrderManage/InsertTransport',
                                       params: {
                                           return_id: return_id,
                                           orc_deliver_code: orc_deliver_code,
                                           orc_deliver_date: orc_deliver_date,
                                           orc_deliver_time: orc_deliver_time,
                                       },
                                       success:function(form,action) {
                                           myMask.hide();
                                           var result = Ext.decode(form.responseText);
                                           if (result.success) {
                                               myMask.hide();
                                               Ext.Msg.alert("提示信息", "物流信息新增成功！");
                                              orderReturnForm.getForm().reset();
                                              transport.hide();
                                              Ext.getCmp('transportBtn').setDisabled(true);
                                           }
                                       }
                                   });
                               }
                               else {
                                   myMask.hide();
                                   Ext.Msg.alert("提示信息", "請填寫正確物流信息！");
                                 
                               }
                           }

                       },
                       //這個是收到商品可退button
            {
                xtype: 'button',
                text: '收到商品可退',
                id: 'couldReturnBtn',
                hidden: true,
                disabled:true,
                handler: function () {
                    var return_id = Ext.getCmp('return_id').getValue();
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    Ext.Ajax.request({
                        url: '/OrderManage/CouldReturn',
                        params: {
                            return_id: return_id,
                            ormpackage:Ext.getCmp('package').getValue(),
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "收到退回的商品");
                                orderReturnForm.getForm().reset();
                                Ext.getCmp('couldReturnBtn').setDisabled(true);
                                Ext.getCmp('package').hide();
                                CouldReturnGrid.hide();

                            }
                            else {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "失敗！");
                            }
                        },
                        failure: function () {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", "出現異常！");
                        }
                    });
                }
            },
                     //這個是確認入庫button
            {
                xtype: 'button',
                text: '確認入庫',
                id: 'wareHouseBtn',
                hidden: true,
                disabled: true,
                handler: function () {
                    var return_id = Ext.getCmp('return_id').getValue();
                    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                    var row = Ext.getCmp("CouldReturnGrid").getSelectionModel().getSelection();
                    var data = "";
                    for (var i = 0; i < row.length; i++) {
                        data += row[i].data.item_id + "*" + row[i].data.buy_num + ";"
                    }
                    //if (data == "") {
                    //    Ext.Msg.alert("提示信息", "請選擇需要退的商品！");
                    //    return;
                    //}
                    myMask.show();
                    Ext.Ajax.request({
                        url: '/OrderManage/CouldWareHouse',
                        params: {
                            return_id: return_id,
                            data:data,
                        },
                        success: function (form, action) {
                            myMask.hide();
                            var result = Ext.decode(form.responseText);
                            if (result.success) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "成功入庫！");
                                Ext.getCmp("wareHouseBtn").setDisabled(true);
                                CouldReturnGrid.hide();
                                orderReturnForm.getForm().reset();
                            }
                            else {
                                Ext.Msg.alert("提示信息", "失敗！");
                            }
                        },
                        failure: function () {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", "出現異常！");
                        }
                    });
                }
            },
                    //這個是確認退款button

               {
                   xtype: 'button',
                   text: '確認退款單',
                   id: 'returnMoney',
                   hidden: true,
                   disabled: true,
                   fieldLabel: '確認退款單',
                   handler: function (event, toolEl, panel) {
                       if (Ext.getCmp('isZIChu').getValue() == "1") {
                           //全是自出
                           Ext.MessageBox.confirm("提示信息", "自出商品請自行聯繫供應商確認退款", function (btn) {
                               if (btn == "yes") {
                                   var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                                   myMask.show();
                                   Ext.Ajax.request({
                                       url: '/OrderManage/CouldReturnMoney',
                                       params: {
                                           order_id: Ext.getCmp('order_id').getValue(),
                                           bank_name: Ext.getCmp('bank_name').getValue(),
                                           bank_branch: Ext.getCmp('bank_branch').getValue(),
                                           bank_account: Ext.getCmp('bank_account').getValue(),
                                           account_name: Ext.getCmp('account_name').getValue(),
                                           bank_note:  Ext.getCmp('bank_note').getValue(),
                                       },
                                       success: function (form, action) {
                                           myMask.hide();
                                           var result = Ext.decode(form.responseText);
                                           if (result.success) {
                                               myMask.hide();
                                               Ext.Msg.alert("提示信息", "退款成功！");
                                               Ext.getCmp('returnMoney').setDisabled(true);
                                               Ext.getCmp('bank_name').hide();
                                               Ext.getCmp('bank_branch').hide();
                                               Ext.getCmp('bank_account').hide();
                                               Ext.getCmp('account_name').hide();
                                               Ext.getCmp('bank_note').hide();
                                               CouldReturnGrid.hide();
                                               orderReturnForm.getForm().reset();
                                           }
                                       },
                                       failure: function () {
                                           myMask.hide();
                                           Ext.Msg.alert("提示信息", "出現異常");
                                       }
                                   });
                               }
                               else {
                                   return false;
                               }
                           });
                       }
                       else {
                           var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                           myMask.show();
                           Ext.Ajax.request({
                               url: '/OrderManage/CouldReturnMoney',
                               params: {
                                   return_id: Ext.getCmp('return_id').getValue(),
                                   bank_name: Ext.getCmp('bank_name').getValue(),
                                   bank_branch: Ext.getCmp('bank_branch').getValue(),
                                   bank_account: Ext.getCmp('bank_account').getValue(),
                                   account_name: Ext.getCmp('account_name').getValue(),
                                   bank_note: Ext.getCmp('bank_note').getValue(),
                               },
                               success: function (form, action) {
                                   var result = Ext.decode(form.responseText);
                                   myMask.hide();
                                   if (result.success) {
                                       Ext.Msg.alert("提示信息", "退款成功！");
                                       Ext.getCmp('returnMoney').setDisabled(true);
                                       CouldReturnGrid.hide();
                                       Ext.getCmp('bank_name').hide();
                                       Ext.getCmp('bank_branch').hide();
                                       Ext.getCmp('bank_account').hide();
                                       Ext.getCmp('account_name').hide();
                                       Ext.getCmp('bank_note').hide();
                                       orderReturnForm.getForm().reset();
                                   }
                               },
                               failure: function () {
                                   myMask.hide();
                                   Ext.Msg.alert("提示信息", "出現異常");
                               }
                           });
                       }
                   }
               },
               ],
           },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textfield',
                        id: 'bank_name',
                        name: 'bank_name',
                        fieldLabel: '銀行帳戶',
                        hidden: true,
                        maxLength:20,
                        labelWidth: 60,
                      
                    },
                    {
                        xtype: 'textfield',
                        id: 'bank_branch',
                        name: 'bank_branch',
                        fieldLabel: '分行',
                        hidden: true,
                        labelWidth: 60,
                        maxLength: 20,
                        margin: '0 0 0 15',
                    },
               
                ],
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                         {
                             xtype: 'textfield',
                             id: 'bank_account',
                             name: 'bank_account',
                             fieldLabel: '帳號',
                             maxLength: 20,
                             hidden: true,
                             labelWidth: 60,
                         },
                    {
                        xtype: 'textfield',
                        id: 'account_name',
                        name: 'account_name',
                        fieldLabel: '戶名',
                        maxLength: 20,
                        hidden: true,
                        labelWidth: 60,
                        margin: '0 0 0 15',
                    },
                ],
            },

            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'textareafield',
                        id: 'bank_note',
                        name: 'bank_note',
                        fieldLabel: '退款資訊 (最多200字)',
                        hidden: true,
                        width: 360,
                        maxLength: 200,
               
                    }
                ],
            },

            {
                   xtype: 'textfield',
                   fieldLabel:'是否全是自出',
                   id: 'isZIChu',
                   hidden:true,
               },
            {
                     xtype: 'textfield',
                     fieldLabel: '付款方式',
                     id: 'order_payment',
                     hidden: true,
                 },
        ],
    });
    //客戶信息
    var orderDeliverForm = Ext.create('Ext.form.Panel', {
        layout: 'anchor',
        width: 600,
        url: '/OrderManage/SaveOrderStatus',
        margin: '0 10 0 0',
        bodyPadding: '15',
        plain: true,
        hidden:true,
        id: 'orderDeliverForm',
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '客戶姓名',
                id: 'orc_name',
                name: 'orc_name',
                allowBlank: false,
                maxLength:10,
                width: 360
            },
            {
                xtype: 'textfield',
                fieldLabel: '客戶電話',
                id: 'orc_phone',
                name: 'orc_phone',
                allowBlank: false,
                maxLength:30,
                width: 360
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                    {
                        xtype: 'combobox',
                        fieldLabel: '收貨地址',
                        id: 'orc_zipcode',
                        name: 'orc_zipcode',
                        editable:false,
                        store: zipStore,
                        lastQuery:'',
                        displayField: 'zipname',
                        valueField: 'zipcode',
                        allowBlank: false,
                        width: 245
                    },
                    {
                        xtype: 'textfield',
                        id: 'orc_address',
                        name: 'orc_address',
                        allowBlank: false,
                        maxLength: 200,
                        width: 200
                    },
                ],
            },
            {
                xtype: 'combobox',
                fieldLabel: '退貨類型',
                id: 'orc_type',
                name: 'orc_type',
                store: typeStore,
                displayField: 'parameterName',
                valueField: 'parameterCode',
                lastQuery:'',
                value: '1',
                editable: false,
            },    
            {
                xtype: 'combobox',
                fieldLabel: '發票處理',
                id: 'invoice_deal',
                name: 'invoice_deal',
                displayField: 'parameterName',
                valueField: 'parameterCode',
                store: InvoiceDealStore,
                lastQuery: '',
                value: '1',
                editable:false,
            },
            {
                xtype: 'textarea',
                fieldLabel: '客戶備註(最多200字)',
                id: 'orc_remark',
                name: 'orc_remark',
                width: 360,
                maxLength: 200,
            },
            {
                xtype: 'textarea',
                fieldLabel: '客服備註(最多200字)',
                id: 'orc_service_remark',
                name: 'orc_service_remark',
                width: 360,
                maxLength: 200,
            },
            {
                xtype: 'radiogroup',
                fieldLabel: '是否需回收',
                id: 'orc_send',
                name: 'orc_send_name',
                width: 200,
                allowBlank:false,
                items: [
                    { id: 'yes', name: 'orc_send_name', boxLabel: '是', inputValue: '1', checked: true },
                        { id: 'no', name: 'orc_send_name', boxLabel: '否', inputValue: '0' },
                ],
            },
        ],
        buttonAlign: 'right',
        buttons: [{
            text: '確認',
            formBind: true,
            disabled: true,
            handler: function () {
                var form = this.up('form').getForm();
                var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "Please wait..." });
                myMask.show();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            return_id: Ext.getCmp('return_id').getValue(),
                            orc_name: Ext.getCmp('orc_name').getValue(),
                            orc_phone: Ext.getCmp('orc_phone').getValue(),
                            orc_zipcode: Ext.getCmp('orc_zipcode').getValue(),
                            orc_address: Ext.getCmp('orc_address').getValue(),
                            orc_remark: Ext.getCmp('orc_remark').getValue(),
                            orc_type: Ext.getCmp('orc_type').getValue(),
                            orc_service_remark: Ext.getCmp('orc_service_remark').getValue(),
                            invoice_deal: Ext.getCmp('invoice_deal').getValue(),
                            orc_send: Ext.getCmp('orc_send').getValue().orc_send_name,
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            myMask.hide();
                            if (result.success) {
                                myMask.hide();
                                Ext.Msg.alert("提示信息", "客戶信息更新成功！");
                                orderReturnForm.getForm().reset();
                                orderDeliverForm.hide();

                            }
                        },
                        failure: function (form, action) {
                            myMask.hide();
                            Ext.Msg.alert("提示信息", "出現異常！");
                           
                        }
                    })
                }
            }
        }
        ]
    });
    //物流信息
    var transport = Ext.create("Ext.form.Panel", {
        layout: 'anchor',
        width: 600,
        url: '/OrderManage/InsertTransport',
        bodyPadding: '15',
        border: false,
        plain: true,
        id: 'transport',
        hidden:true,
        items: [
            {
                xtype: 'textfield',
                fieldLabel: '物流編號',
                id: 'orc_deliver_code',
                name: 'orc_deliver_code',
                allowBlank: false,
                maxLength:50,
                width: 360
            },
            {
                xtype: 'fieldcontainer',
                layout: 'hbox',
                items: [
                {
                    xtype: 'datefield',
                    fieldLabel: '預計收貨時間段',
                    id: 'orc_deliver_date',
                    name: 'orc_deliver_date',
                    allowBlank: false,
                    width: 360,
                    editable: false,
                    listeners: {
                        select: function () {
                            var date =new Date(Ext.getCmp('orc_deliver_date').getValue());
                            var now_date = new Date(new Date().getFullYear(), new Date().getMonth(), new Date().getDate());
                            if (date <= now_date)
                            {
                                Ext.Msg.alert("提示信息", "預計退貨時間需大於今日");
                                Ext.getCmp('orc_deliver_date').setValue()
                            }
                        }
                    },
                },
                {
                    xtype: 'combobox',
                    store: DateStore,
                    displayField: 'txt',
                    valueField: 'value',
                    id: 'orc_deliver_time',
                    name: 'orc_deliver_time',
                    value: 0,
                    editable:false,
                    allowBlank: false,
                }],            
            },
        ],
 
    });
    //唯一的grid負責承載收到商品可退、確認入庫，確認退款單
    var CouldReturnGrid = Ext.create('Ext.grid.Panel', {
        id: 'CouldReturnGrid',
        store:CouldReturnStore,
        columnLines: true,
        hidden:true,
        frame: true,
        flex: 6.1,
        columns: [
            { header: "訂單編號", dataIndex: 'order_id', width: 150, align: 'center' },
            { header: "商品細項編號", dataIndex: 'item_id', width: 150, align: 'center' },
            { header: "商品名稱", dataIndex: 'product_name', width: 150, align: 'center' },
            { header: "規格", dataIndex: 'product_spec_name', width: 120, align: 'center' },
            { header: "數量", dataIndex: 'buy_num', width: 120, align: 'center' },
            { header: "價格", dataIndex: 'single_money', width: 120, align: 'center' },
            { header: "出貨方式", dataIndex: 'product_mode', width: 120, align: 'center' }        
        ],
        //bbar: Ext.create('Ext.PagingToolbar', {
        //    store: CouldReturnStore,
        //    pageSize: 25,
        //    displayInfo: true,
        //    displayMsg: "當前顯示記錄" + ': {0} - {1}' + "總計" + ': {2}',
        //    emptyMsg: "沒有記錄可以顯示"
        //}),
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'vbox',
        items: [orderReturnForm, buttonForm,orderDeliverForm,transport, CouldReturnGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                CouldReturnGrid.width = 950;
                CouldReturnGrid.height = 220;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //QueryAuthorityByUrl('/OrderManage/OrderReturnList');
});