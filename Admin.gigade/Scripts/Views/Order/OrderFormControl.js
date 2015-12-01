var OrderName = {
    xtype: 'fieldcontainer',
    labelWidth: 65,
    layout: 'hbox',
    defaultType: 'textfield',
    items: [{
        fieldLabel: USERID,//會員編號
        name: 'd_userID',
        id: 'd_userID',
        labelWidth: 65,
        width: 200,
        vtype: 'regxUserID',
        allowBlank: false,
    }, { //add by wwei0216w 添加性別選項 2015/1/21
        xtype: 'radiogroup',
        id: 's_checkActionSex',
        labelWidth: 45,
        fieldLabel: GENDER,
        width: 150,
        margin: '0 0 0 10',
        defaults: {
            name: 's_checkActionSex'
        },
        columns: 2,
        vertical: true,
        items: [
        { boxLabel: BOY, inputValue: '1' },
        { boxLabel: GIRL, inputValue: '0', checked: true }, ]
    }]
};

//賣場名稱
var outsiteStore = Ext.create('Ext.data.Store', {
    fields: ['channel_id', 'channel_name_full', 'channel_type', 'receipt_to'],
    proxy: {
        type: 'ajax',
        url: '/Order/GetChannel',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    },
    autoLoad: true
});

//訂單狀態
var orderStatusStore = Ext.create('Ext.data.Store', {
    id: 'orderStatusStore',
    autoDestroy: true,
    fields: ['parameterCode', 'parameterName'],
    autoLoad: false,
    data: [
         { parameterCode: '2', parameterName: ORDER_STATUS_WAIT_DELIVER },
         { parameterCode: '3', parameterName: ORDER_STATUS_DELIVER_NOW },
         { parameterCode: '4', parameterName: ORDER_STATUS_DELIVER_SUCCESS }
    ]
});

//會員編號  add by zhuoqin0830w  2015/11/12
Ext.apply(Ext.form.field.VTypes, {
    regxUserID: function (val, field) {
        return /^[1-9]\d*$/.test(val);
    },
    regxUserIDText: FORMAT_ERROR
});