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
    items: [{
        fieldLabel: CNFULLNAME,
        name: 'b_txtName',
        submitValue: false,
        labelWidth: 65,
        width: 200,
        id: 'txtCNFullName',
        xtype: 'combobox',
        allowBlank: false,
        store: nameStore,
        enableKeyEvents: true,
        queryCaching: true,
        hideTrigger: true,
        displayField: 'user_name',
        valueField: 'user_id',
        queryMode: 'local',
        typeAhead: true,
        autoScroll: false,
        maxheight: 200,
        hiddenName: 'selectUserID',
        listConfig: {
            getInnerTpl: function () {
                return '<h4>{user_name} ({user_email})</h4>';
            },
            resizable: function () {
                return true;
            }
        },
        forceSelection: true,
        listeners: {
            select: function (combo, record) {
                Ext.getCmp('txtActionPhone').setValue(record[0].data.user_mobile);
                Ext.getCmp('b_txtAddress').setValue(record[0].data.user_address);
                Ext.getCmp("s_checkActionSex").setValue({ 's_checkActionSex': record[0].data.user_gender });
                //add by zhuoqin0830w  2015/08/21  將選中的combobox數據的userid賦值給 隱藏的控件
                Ext.getCmp("selectUserID").setValue(record[0].data.user_id);
            },
            change: function () {
                var condition = Ext.getCmp("txtCNFullName").getRawValue();
                if (condition != "") { // && nameStore.find("user_name", condition) < 0 
                    Ext.Ajax.request({
                        url: '/Order/GetInfoByTest',
                        method: 'post',
                        params: {
                            condition: condition
                        },
                        success: function (form, action) {
                            var result = Ext.decode(form.responseText);
                            if (result.items.length > 0) {
                                nameStore.loadData(result.items);
                            } else {
                                Ext.getCmp('txtActionPhone').setValue("");
                                Ext.getCmp('b_txtAddress').setValue("");
                            }

                        }
                    });
                }
            }
        }
    }, {
        xtype: 'radiogroup',
        id: 's_checkActionSex',
        labelWidth: 45,
        fieldLabel: GENDER,
        margin: '0 0 0 20',
        width: 150,
        defaults: {
            name: 's_checkActionSex'
        },
        columns: 2,
        vertical: true,
        items: [{ boxLabel: BOY, inputValue: '1', },
        { boxLabel: GIRL, inputValue: '0', checked: true }]
    }, {
        //add by zhuoqin0830w  2015/08/21  
        //添加一個隱藏的 textfile控件  以存儲 combobox 選中後的數據 而不是 默認第一行的數據
        submitValue: false,
        hidden: true,
        id: 'selectUserID'
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
    data: [
         { parameterCode: '2', parameterName: ORDER_STATUS_WAIT_DELIVER },
         { parameterCode: '3', parameterName: ORDER_STATUS_DELIVER_NOW },
         { parameterCode: '4', parameterName: ORDER_STATUS_DELIVER_SUCCESS },
         { parameterCode: '0', parameterName: ORDER_STATUS_WAIT_PAYMENT }
    ]
});