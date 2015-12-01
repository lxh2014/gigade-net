var nameStore = Ext.create('Ext.data.Store', {
    fields: ['user_id', 'user_email', 'user_name', 'user_mobile', 'user_phone', 'user_address', 'user_gender'],
    proxy: {
        type: 'ajax',
        url: '/Order/GetInfoByTest',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    autoLoad: false
});

var OrderName = {
    xtype: 'fieldcontainer',
    labelWidth: 65,
    layout: 'hbox',
    defaultType: 'textfield',
    items: [{//edit by zhuoqin0830w  2015/11/12
        fieldLabel: USERID,//會員編號
        name: 'd_userID',
        id: 'd_userID',
        labelWidth: 65,
        width: 200,
        vtype: 'regxUserID',
        allowBlank: false,
        listeners: {
            specialkey: function (field, e) {
                if (e.getKey() == e.ENTER) {
                    var condition = Ext.getCmp("d_userID").getValue();
                    if (condition != "") {
                        Ext.Ajax.request({
                            url: '/Order/GetInfoByTest',
                            method: 'post',
                            params: {
                                condition: condition
                            },
                            success: function (form, action) {
                                var result = Ext.decode(form.responseText);
                                if (result.items.length > 0) {
                                    Ext.getCmp("txtCNFullName").setValue(result.items[0].user_name);
                                    Ext.getCmp('txtActionPhone').setValue(result.items[0].user_mobile);
                                    Ext.getCmp('b_txtAddress').setValue(result.items[0].user_address);
                                    Ext.getCmp("s_checkActionSex").setValue({ 's_checkActionSex': result.items[0].user_gender });
                                    Ext.getCmp("d_userEmail").setValue(result.items[0].user_email);
                                } else {
                                    Ext.getCmp('txtActionPhone').setValue("");
                                    Ext.getCmp('b_txtAddress').setValue("");
                                }
                            }
                        });
                    }
                }
            }
        }
    }, {
        xtype: 'radiogroup',
        id: 's_checkActionSex',
        labelWidth: 45,
        fieldLabel: GENDER,
        margin: '0 0 0 10',
        width: 150,
        defaults: {
            name: 's_checkActionSex'
        },
        columns: 2,
        vertical: true,
        items: [{ boxLabel: BOY, inputValue: '1', },
        { boxLabel: GIRL, inputValue: '0', checked: true }]
    }]
};

//買場名稱
var outsiteStore = Ext.create('Ext.data.Store', {
    fields: ['channel_id', 'channel_name_full', 'channel_type', 'receipt_to'],
    // 將 "receipt_to": "1"  改為 "receipt_to": "0"  edit by zhuoqin0830w  2015/05/21
    data: [{ "channel_id": "1", "channel_name_full": GIGADE, "channel_type": "2", "receipt_to": "0" }],//吉甲地
    autoLoad: true
});

//訂單狀態
var orderStatusStore = Ext.create('Ext.data.Store', {
    id: 'orderStatusStore',
    autoDestroy: true,
    fields: ['parameterCode', 'parameterName'],
    autoLoad: false,
    data: [{ parameterCode: '2', parameterName: ORDER_STATUS_WAIT_DELIVER },
        { parameterCode: '3', parameterName: ORDER_STATUS_DELIVER_NOW },
        { parameterCode: '4', parameterName: ORDER_STATUS_DELIVER_SUCCESS },
        { parameterCode: '0', parameterName: ORDER_STATUS_WAIT_PAYMENT }]
});

//會員編號  add by zhuoqin0830w  2015/11/12
Ext.apply(Ext.form.field.VTypes, {
    regxUserID: function (val, field) {
        return /^[1-9]\d*$/.test(val);
    },
    regxUserIDText: FORMAT_ERROR
});