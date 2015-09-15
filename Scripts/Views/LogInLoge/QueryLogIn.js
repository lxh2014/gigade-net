
var pageSize = 20;
Ext.define('GIGADE.LogInLoge', {
    extend: 'Ext.data.Model',
    fields: [{ name: 'user_id', type: 'uint' },
            { name: 'user_username', type: 'string' },
            { name: 'login_id', type: 'uint' },
            { name: 'login_ipfrom', type: 'string' },
            { name: 'Strlogindate', type: 'string'}]
});

var logInLogeStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.LogInLoge',
    pageSize: pageSize,
    autoDestroy: true,
    proxy: {
        type: 'ajax',
        url: '/LogInLoge/QueryLogIn',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});


var titlePanel = Ext.create('Ext.panel.Panel', {
    border: 0,
    width: 300,
    height: 50,
    html: '<h1>系統登入記錄</h1>',
    bodyStyle: 'padding:20px 0px 15px 25px; font-size:15px'
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp('logInGrid').down('#btnEdit').setDisabled(selections.length == 0);
        }
    }
});

function formatDate(value) {
    var dt = new Date();
    dt = Ext.Date.parse(value, 'Y-m-d H:i:s');
    var c = Ext.Date.dateFormat(dt, "Y-m-d H:i:s");
    return c;
}

Ext.onReady(function () {

    var logInGrid = Ext.create('Ext.grid.Panel', {
        id: 'logInGrid',
        store: logInLogeStore,
        selModel: sm,
        height: document.documentElement.clientHeight - 65,
        columns: [{ header: '流水號', dataIndex: 'login_id', width: 100, align: 'center' },
                  { header: '使用者編號', dataIndex: 'user_id', editor: { xtype: 'numberfield' }, width: 100, align: 'center' },
                  { header: '名稱', dataIndex: 'user_username', editor: { xtype: 'textfield', allowBlank: false, blankText: 'Is empty!' }, width: 180, align: 'center' },
                  { header: '來源', dataIndex: 'login_ipfrom', width: 280, align: 'center' },
                          {
                              header: '登入時間',
                              dataIndex: 'Strlogindate',
                              //renderer: formatDate,
                              editor: {
                                  xtype: 'datefield',
                                  disabledDays: [0, 6],
                                  format: 'Y-m-d H:i:s',
                                  disabledDaysText: 'The days is disabled!'
                              },
                              width: 280, align: 'center'
                          }
                ],
        tbar: [
                { xtype: 'button', id: 'btnAdd', text: '新增', iconCls: 'icon-add', handler: onAddClick },
                { xtype: 'button', id: 'btnEdit', text: '編輯', disabled: true, iconCls: 'icon-edit', handler: onEditClick }
              ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: logInLogeStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: '分頁{0} - {1} of {2}',
            emptyMsg: 'No topics to display',
            prevText: '上一頁',
            nextText: '下一頁',
            refreshText: '刷新',
            lastText: '尾頁',
            firstText: '首頁',
            beforePageText: '當前頁',
            afterPageText: '共{0}頁'

        }),
        plugins: [
            Ext.create('Ext.grid.plugin.CellEditing', {
                clicksToEdit: 2
            })
        ]
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'anchor',
        items: [titlePanel, logInGrid],
        renderTo: Ext.getBody()
    });

    logInLogeStore.load({ params: { start: 0, limit: pageSize} });
});

onAddClick = function () {
    saveWin();
}

onEditClick = function () {
    var row = Ext.getCmp('logInGrid').getSelectionModel().getSelection();
    if (row.length <= 0) {
        Ext.Msg.alert('', '0');
    } else if (row.length > 1) {
        Ext.Msg.alert('', '1');
    } else if (row.length == 1) {
        saveWin(row[0]);
    }

}

//自定義驗證Type
Ext.apply(Ext.form.field.VTypes, {
    IPAddress: function (v) {
        return /^\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}$/.test(v);
    },
    IPAddressText: 'IP格式錯誤!'
});


function saveWin(row) {
    var saveForm = Ext.create('Ext.form.Panel', {
        id: 'saveForm',
        border: 0,
        bodyStyle: 'padding:5px',
        defaultType: 'textfield',
        labelWidth: 45,
        defaults: { anchor: '95%', msgTarget: 'side', labelAlign: 'top' },
        items: [
                { fieldLabel: 'Name', id: 'userName', name: 'user_username', allowBlank: false, blankText: '用戶名不能為空!', submitValue: false },
                { fieldLabel: 'IP', id: 'userIP', name: 'login_ipfrom', vtype: 'IPAddress', allowBlank: false, blankText: 'IP地址不能為空!', submitValue: false },
                { fieldLabel: 'Date', id: 'userDate', name: 'Strlogindate', allowBlank: false, blankText: '登錄時間不能為空!', submitValue: false },
                { fieldLabel:'date',xtype:'datefield',format:'Y-m-d H:i:s'}
               ],
        buttonAlign: 'center',
        buttons: [{
            text: 'Save',
            disabled: true,
            formBind: true,
            handler: function () {
                logInLogeStore.load();
                Ext.getCmp("saveWin").close();
            }
        }, {
            text: 'Cancel',
            handler: function () {
                Ext.getCmp("saveWin").close();
            }
        }]
    });

    Ext.create('Ext.window.Window', {
        id: 'saveWin',
        layout: 'fit',
        closeAction: 'destroy',
        width: 300,
        height: 350,
        resizable: false,
        modal: true,
        //draggable:false,
        items: [saveForm],
        iconCls: row ? 'icon-edit' : 'icon-add',
        listeners: {
            'show': function () {
                if (row) {
                    saveForm.getForm().loadRecord(row);
                } else {
                    saveForm.getForm().reset();
                }
            }
        }

    }).show();

}


function editFunction() {

}