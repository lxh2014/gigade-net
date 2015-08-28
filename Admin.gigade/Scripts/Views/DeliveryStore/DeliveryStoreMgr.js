var pageSize = 25;

Ext.define('GIGADE.DeliveryStore', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'rowid', type: 'string' },
        { name: 'delivery_store_id', type: 'string' },
        { name: 'delivery_store_name', type: 'string' },
        { name: 'big', type: 'string' },
        { name: 'bigcode', type: 'string' },
        { name: 'middle', type: 'string' },
        { name: 'middlecode', type: 'string' },
        { name: 'small', type: 'string' },
        { name: 'smallcode', type: 'string' },
        { name: 'store_id', type: 'string' },
        { name: 'store_name', type: 'string' },
        { name: 'address', type: 'string' },
        { name: 'phone', type: 'string' },
        { name: 'status', type: 'string' }
    ]
});
//超商店家
var deliveryStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.DeliveryStore',
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/QueryDeliveryStore',
        actionMethods: 'post',
        reader: {
            type: 'json',
            totalProperty: 'totalCount',
            root: 'data'
        }
    }
});
deliveryStore.on("beforeload", function () {

    Ext.apply(deliveryStore.proxy.extraParams,
         {
             Delivery: Ext.getCmp('delivery_store').getValue(),
             Status: Ext.getCmp('s_status').getValue()
         });
});

//物流業者
var paraDelivery = Ext.create('Ext.data.Store', {
    fields: ['ParameterCode', 'parameterName'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/QueryDelivery',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

//地區別
var bigZip = Ext.create('Ext.data.Store', {
    fields: ['big', 'bigcode'],
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/DeliveryStore/QueryBig',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
var middleZip = Ext.create('Ext.data.Store', {
    fields: ['middle', 'middlecode'],
    proxy: {
        type: 'ajax',
        url: '/Channel/QueryCity',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
var smallZip = Ext.create('Ext.data.Store', {
    fields: ['small', 'zipcode'],
    proxy: {
        type: 'ajax',
        url: '/Channel/QueryZip',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});

var s_status = Ext.create('Ext.data.Store', {
    fields: ['name', 'value'],
    data: [
        { name: STATUS_ALL, value: '0' },
        { name: STATUS_NORMAL, value: '1' },
        { name: STATUS_CLOASE, value: '2' },
        { name: STATUS_NO_DELIVERY, value: '3' }
    ]
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("grid").down('#remove').setDisabled(selections.length == 0);
            Ext.getCmp("grid").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 60,
        width: 900,
        defaults: { anchor: "95%" },
        labelWidth: 45,
        border: false,
        plain: true,
        items: [{
            xtype: 'panel',
            layout: 'column',
            border: 0,
            items: [{
                border: 0,
                bodyStyle: 'padding:5px 5px 0px 5px',
                items: [{
                    xtype: 'combobox',
                    store: paraDelivery,
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    queryMode: 'local',
                    fieldLabel: DELIVERY_STORE_NAME,
                    editable: false,
                    id: 'delivery_store'
                }]
            }, {
                border: 0,
                bodyStyle: 'padding:5px 5px 0px 5px',
                items: [{
                    xtype: 'combobox',
                    displayField: 'name',
                    valueField: 'value',
                    id: 's_status',
                    editable: false,
                    fieldLabel: STORE_STATUS,
                    store: s_status
                }]
            }, {
                border: 0,
                bodyStyle: 'padding:5px 5px 0px 5px',
                items: [{
                    xtype: 'button',
                    iconCls: 'icon-search',
                    text: SEARCH,
                    handler: query
                }]
            }]
        }]
    });

    var grid = Ext.create('Ext.grid.Panel', {
        id: "grid",
        store: deliveryStore,
        width: document.documentElement.clientWidth,
        height: 710,
        columnLines: true,
        frame: true,
        columns: [
            { header: NUMBER, xtype: 'rownumberer', width: 100, align: 'center' },
            { header: STORE_ID, dataIndex: 'store_id', width: 120, align: 'center' },
            { header: STORE_NAME, dataIndex: 'store_name', width: 150, align: 'center' },
            { header: STORE_ADDRESS, dataIndex: 'address', width: 180, align: 'left' },
            { header: STORE_PHONE, dataIndex: 'phone', width: 120, align: 'center' },
            { header: STORE_STATUS, dataIndex: 'status', width: 120, align: 'center', renderer: Status },
            { header: DELIVERY_STORE_NAME, dataIndex: 'delivery_store_name', width: 150, align: 'center' },
            { header: BIG_TEXT, dataIndex: 'big', width: 120, align: 'center' },
            { header: MIDDLE_TEXT, dataIndex: 'middle', width: 120, align: 'center' },
            { header: SMALL_TEXT, dataIndex: 'small', width: 120, align: 'center' }
        ],
        tbar: [
            { xtype: 'button', id: 'add', text: ADD, iconCls: 'icon-add', hidden: true, handler: onAddClick },
            { xtype: 'button', id: 'edit', text: EDIT, iconCls: 'icon-edit', hidden: true, disabled: true, handler: onEditClick },
            { xtype: 'button', id: 'remove', text: REMOVE, iconCls: 'icon-remove', hidden: true, disabled: true, handler: onRemoveClick },
            '->',
            { text: ' ' }
        ],
        listeners: {
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        bbar: Ext.create('Ext.PagingToolbar', {
            store: deliveryStore,
            pageSize: pageSize,
            displayInfo: true
        }),
        selModel: sm
    });

    Ext.create('Ext.Viewport', {
        layout: 'anchor',
        items: [frm, grid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                grid.width = document.documentElement.clientWidth;
                this.doLayout();

            }
        }
    });

    ToolAuthority();
    deliveryStore.load();

});
function Status(val) {
    switch (val) {
        case "1":
            return STATUS_NORMAL;
            break;
        case "2":
            return STATUS_CLOASE;
            break;
        case "3":
            return STATUS_NO_DELIVERY;
            break;
    }
}

onAddClick = function () {
    SaveWin();
}
onEditClick = function () {
    var row = Ext.getCmp("grid").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        SaveWin(row[0]);
    }
}
onRemoveClick = function () {
    var row = Ext.getCmp("grid").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.rowid + '|';
                }
                Ext.Ajax.request({
                    url: '/DeliveryStore/DeleteDeliveryStore',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            deliveryStore.load();
                        }
                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
        });
    }
}

//查询
query = function () {

    deliveryStore.removeAll();
    Ext.getCmp("grid").store.loadPage(1, {
        params: {
            Delivery: Ext.getCmp('delivery_store').getValue(),
            Status: Ext.getCmp('s_status').getValue()
        }
    });
}