showFunction = function (row, store) {
    //物流業者
    Ext.define("gigade.paraModel", {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'parameterCode', type: 'string' },
            { name: 'parameterName', type: 'string' }
        ]
    });
    var DeliverStore = Ext.create("Ext.data.Store", {
        model: 'gigade.paraModel',
        autoLoad: true,
        proxy: {
            type: 'ajax',
            url: '/OrderManage/QueryPara?paraType=Deliver_Store',
            noCache: false,
            getMethod: function () { return 'get'; },
            actionMethods: 'post',
            reader: {
                type: 'json',
                root: 'items'
            }
        }
    });
    var editFrm = Ext.create('Ext.form.Panel', {
        id: 'editFrm',
        frame: true,
        plain: true,
        constrain: true,
        defaultType: 'textfield',
        autoScroll: true,
        layout: 'anchor',
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [{
            xtype: 'displayfield',
            fieldLabel: '出貨流水號',
            id: 'expect_id',
            name: 'expect_id'
        },
        {
            xtype: 'displayfield',
            name: 'slave_id',
            id: 'slave_id',
            maxLength: 25,
            fieldLabel: '廠商出貨單號'
        },
        {
            xtype: 'fieldcontainer',
            fieldLabel: '狀態',
            id: 'validate',
           
            defaultType: 'radiofield',
            submitValue: true,
            defaults: {
                flex: 1
            },
            layout: 'hbox',
            items: [{
                boxLabel: '未出貨',
                name: 'status',
                inputValue: '0',
                readOnly: true,
                id: 'radio1',
                checked: true
            }, {
                boxLabel: '已出貨',
                name: 'status',
                readOnly: true,
                inputValue: '1',
                id: 'radio2'
            },
            {
                boxLabel: '異常',
                name: 'status',
                readOnly: true,
                inputValue: '2',
                id: 'radio3'
            }]
        }, {
            xtype: 'combobox',
            allowBlank: true,
            fieldLabel: '物流業者',
            hidden: false,
            id: 'store',
            name: 'store',
            store: DeliverStore,
            displayField: 'parameterName',
        
            valueField: 'parameterCode',
            typeAhead: true,
            forceSelection: false,
            readOnly: true,
            editable: false,
            value: 0
        }, {
            xtype: 'displayfield',
            name: 'code',
            id: 'code',
            fieldLabel: '物流單號'
        },
        {
            xtype: 'displayfield',
            name: 'stime',
            id: 'stime',
            fieldLabel: '出貨時間'
        }, {
            xtype: 'displayfield',
            name: 'date_one',
            id: 'date_one',
            fieldLabel: '建立時間'
        }, {
            xtype: 'displayfield',
            fieldLabel: '更新時間',
            id: 'date_two',
            name: 'date_two',
        },
        {
            xtype: 'displayfield',
            fieldLabel: '出貨單備註',
            id: 'note'
        }

        ]

    });
    var editWin = Ext.create('Ext.window.Window', {
        title: '出貨單',
        //        iconCls: 'icon-user-edit',
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
                editFrm.getForm().loadRecord(row);
            }
        }
    });
    editWin.show();

}
