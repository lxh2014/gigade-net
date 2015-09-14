//OrderStatusModify
Ext.onReady(function () {

    var FunctionForm = Ext.create('Ext.form.Panel', {
        frame: false,
        title: '待付款單轉出貨單',
        bodyStyle: 'padding:5px 0px 0',
        border:0,
        width: 400,
        layout: 'anchor',
        fieldDefaults: {
            msgTarget: 'side',
            labelWidth: 150
        },
        defaultType: 'textfield',
        defaults: {
            anchor: '100%'
        }, items: [
            {
                xtype: 'panel',
                bodyStyle: "padding:5px;background:#87ceeb",//background:#87CEEB
                border: false,
                margin: 'padding:5 0 10 5',
                html: "注意事項：此功能不適用於訂已被取消的單子，<br/>如果要執行取消單子需把原本還她的購物金扣掉。 "
                
            },
            {
                fieldLabel: '付款單號',
                name: 'order_id',
                id: 'order_id',
                allowBlank: false,
                xtype: 'numberfield'
            },{
                fieldLabel: '紅利折抵',
                name: 'deduct_card_bonus',
                id: 'deduct_card_bonus',
                xtype: 'numberfield',
                minValue: 0,
                value:0
            },
            {
                xtype: 'radiogroup',
                id: 'isBilling_checked',
                name: 'isBilling_checked',
                fieldLabel: "是否已經對賬",
                width: 150,
                defaultType: 'radio',
                columns: 2,
                defaults: {
                    name: 'isBilling_checked'
                },
                vertical: true,
                items: [
                    { id: 'billing_checked_id1', boxLabel: "否", inputValue: 'false', checked: true },
                    { id: 'billing_checked_id2',  boxLabel: "是", inputValue: 'true' }
                ]
            }
            ,
            {
                xtype: 'radiogroup',
                id: 'isHGBonus',
                name: 'isHGBonus',
                fieldLabel: "是否發放購物金/HG點數",
                width: 150,
                defaultType: 'radio',
                columns: 2,
                defaults: {
                    name: 'isHGBonus'
                },
                vertical: true,
                items: [
                    { id: 'HGBonus_id1', boxLabel: "否", inputValue: 'false', checked: true },
                    { id: 'HGBonus_id2',  boxLabel: "是", inputValue: 'true' }
                ]
            },
            {
                xtype: 'radiogroup',
                id: 'isCash_record_bonus',
                name: 'isCash_record_bonus',
                fieldLabel: "是否扣除消費者抵用購物金",
                width: 150,
                defaults: {
                    name: 'isCash_record_bonus'
                },
                defaultType: 'radio',
                columns: 2,
                vertical: true,
                items: [
                    { id: 'cash_record_bonus_id1',  boxLabel: "否", inputValue: 'false', checked: true },
                    { id: 'cash_record_bonus_id2',  boxLabel: "是", inputValue: 'true' }
                ]
            }
        ],

        buttons: [{
            text: '開始轉單',
            formBind: true,
            disabled: true,
            handler: function () {
                //alert(Ext.getCmp("isCash_record_bonus").getValue()["isCash_record_bonus"]);
                Ext.Ajax.request({
                    
                    url: '/Order/OrderMasterChangeStatusFromPayToDel',
                    params: {
                        order_id: Ext.htmlEncode(Ext.getCmp('order_id').getValue()),
                        deduct_card_bonus: Ext.htmlEncode(Ext.getCmp("deduct_card_bonus").getValue()),
                        isBilling_checked: Ext.htmlEncode(Ext.getCmp("isBilling_checked").getValue()["isBilling_checked"]),
                        isHGBonus: Ext.htmlEncode(Ext.getCmp("isHGBonus").getValue()["isHGBonus"]),
                        isCash_record_bonus: Ext.htmlEncode(Ext.getCmp("isCash_record_bonus").getValue()["isCash_record_bonus"])
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        
                        Ext.Msg.alert("提示",result.data);
                    },
                    failure: function () {
                        Ext.Msg.alert("提示", "不好，轉單失敗！");
                    }
                });
            }
        }]
    });

    FunctionForm.render(document.body);
    
});