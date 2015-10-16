Ext.Loader.setConfig({ enabled: true });
Ext.Loader.setPath('Ext.ux', '/Scripts/Ext4.0/ux');
Ext.require([
    'Ext.form.Panel',
    'Ext.ux.form.MultiSelect',
    'Ext.ux.form.ItemSelector'
]);
var excels = ['xls', 'xlsx']; //['xls', 'xlsx'];

Ext.apply(Ext.form.field.VTypes, {
    excelFilter: function (val, field) {
        var type = val.split('.')[val.split('.').length - 1].toLocaleLowerCase();
        for (var i = 0; i < excels.length; i++) {
            if (excels[i] == type) {
                return true;
            }
        }
        return false;
    },
    excelFilterText: FILE_TYPE_WRONG

});
var pageSize = 25;
Ext.define('gigade.Bank', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "id", type: "int" },
        { name: "bank_code", type: "string" },
        { name: "bank_name", type: "string" },
        { name: "muser", type: "int" },
        { name: "muser_name", type: "string" },
        { name: "mdate", type: "string" },
        { name: "status", type: "int" }
    ]
});

var VendorBankStore = Ext.create('Ext.data.Store', {
    pageSize: pageSize,
    model: 'gigade.Bank',
    proxy: {
        type: 'ajax',
        url: '/Vendor/GetVendorBankList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
VendorBankStore.on('beforeload', function () {
    Ext.apply(VendorBankStore.proxy.extraParams, {
        key: Ext.getCmp('key').getValue()
    });
});

function Query() {
    Ext.getCmp("gdBank").store.loadPage(1);
}
Ext.onReady(function () {

    var sm = Ext.create('Ext.selection.CheckboxModel', {
        listeners: {
            selectionchange: function (sm, selections) {
                Ext.getCmp("gdBank").down('#edit').setDisabled(selections.length == 0);
            }
        }
    });
    var gdBank = Ext.create('Ext.grid.Panel', {
        id: 'gdBank',
        store: VendorBankStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "銀行代碼", dataIndex: 'bank_code', width: 100, align: 'center' },
            { header: "銀行名稱", dataIndex: 'bank_name', width: 500, align: 'center' },
            {
                header: "修改人", dataIndex: 'muser_name', width: 150, align: 'center'
            },
            { header: "修改時間", dataIndex: 'mdate', width: 180, align: 'center' },
        {
            header: "狀態",
            dataIndex: 'status',
            id: 'status',
            align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "<a href='javascript:void(0);' onclick='onUpdateActive(" + record.data.id + ")'><img hidValue='0' id='img" + record.data.id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                } else {
                    return "<a href='javascript:void(0);' onclick='onUpdateActive(" + record.data.id + ")'><img hidValue='1' id='img" + record.data.id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                }
            }
        }

        ],
        tbar: [
               { xtype: 'button', text: ADD, id: 'add', iconCls: 'icon-user-add', handler: onAddClick },
               { xtype: 'button', text: EDIT, id: 'edit', iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
               { xtype: 'button', text: "匯入", id: 'import', iconCls: 'icon-upload-img', handler: onImportClick },

                '->',
{
    xtype: 'textfield',
    fieldLabel: '關鍵字',
    id: 'key',
    name: 'key',
    margin: '0 5 0 0',
    labelWidth: 60,
    width: 260,
    listeners: {
        specialkey: function (field, e) {
            if (e.getKey() == e.ENTER) {
                Query();
            }
        }
    }
},
               {
                   text: SEARCH, iconCls: 'icon-search', id: 'btnQuery', handler: Query
               },
               {
                   text: '重置', id: 'reset', iconCls: 'ui-icon ui-icon-reset', handler: function () {
                       var code = Ext.getCmp('key');
                       code.setValue('');

                   }
               }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: VendorBankStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
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
    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdBank],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdBank.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();


});



onImportClick = function () {
    var frm = Ext.widget('form', {
        id: 'frm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        url: '/Vendor/ImportVendorBank',
        items: [
              {
                  xtype: 'displayfield',
                  value: '提示:<br/>1.請匯入格式為xls、xlsx的excel文件<br/>2.首行為欄位名,不予匯入<br/>3.資料重複時,略過不處理'
              },
            {
                xtype: 'filefield',
                name: 'importFile',
                id: 'importFile',
                buttonText: '瀏覽..',
                emptyText: '選擇欲匯入之excel',
                width: 300,
                submitValue: true,
                fileUpload: true,
                allowBlank: false,
                vtype: 'excelFilter'
            }],
        buttons: [
                {
                    text: "確認",
                    formBind: true,
                    id: 'sure',
                    disabled: true,
                    handler: function () {
                        var form = Ext.getCmp('frm').getForm();
                        if (form.isValid()) {
                            this.disable();
                            form.submit({
                                waitMsg: FILE_UPLOADING,
                                waitTitle: WAIT_TITLE,
                                success: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.success) {
                                        if (result.error == "0") {
                                            Ext.Msg.alert(INFORMATION, "數據匯入成功！");

                                        }
                                        else {
                                            if (result.error.split(',').length > 5) {
                                                Ext.Msg.alert(INFORMATION, "代碼:<br/>" + result.error.substring(0, 39) + ",……重複！");
                                            }
                                            else {
                                                Ext.Msg.alert(INFORMATION, "代碼:" + result.error + "重複！");
                                            }
                                        }
                                        VendorBankStore.load();
                                        importWin.close();
                                    } else {
                                        Ext.Msg.alert(INFORMATION, "數據匯入失敗,請檢查文件格式！");
                                        Ext.getCmp('sure').setDisabled(false);
                                    }

                                },
                                failure: function (form, action) {
                                    var result = Ext.decode(action.response.responseText);
                                    if (result.error != "") {
                                        Ext.Msg.alert(INFORMATION, result.error);
                                    }
                                    else {
                                        Ext.Msg.alert(INFORMATION, "數據匯入失敗,請聯繫IT人員！");
                                    }
                                    Ext.getCmp('sure').setDisabled(false);
                                }
                            });

                        }
                    }
                }
        ]
    });

    var importWin = Ext.create('Ext.window.Window', {
        title: "銀行信息匯入",
        id: 'importWin',
        iconCls: 'icon-upload-img',
        width: 350,
        height: 200,
        layout: 'fit',
        items: [frm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: '是否關閉',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('importWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                frm.getForm().reset();
            }
        }
    });
    importWin.show();


}

onUpdateActive = function (id) {

    var activeValue = $("#img" + id).attr("hidValue");
    $.ajax({
        url: "/Vendor/UpdateActiveBank",
        data: {
            "id": id,
            "active": activeValue
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (activeValue == 1) {
                $("#img" + id).attr("hidValue", 0);
                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                VendorBankStore.load();
            }
            else {
                $("#img" + id).attr("hidValue", 1);
                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                VendorBankStore.load();
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
            VendorBankStore.load();
        }
    });
}

//編輯銀行代碼或名稱
editBankFunction = function (row) {
    var editBankFrm = Ext.create('Ext.form.Panel', {
        id: 'editBankFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        autoScroll: true,
        labelWidth: 45,
        url: '/Vendor/SaveVendorBank',
        defaults: { anchor: "95%", msgTarget: "side", labelWidth: 80 },
        items: [
                {
                    xtype: 'textfield',
                    id: 'id',
                    name: 'id',
                    hidden: true
                },
               {
                   xtype: 'textfield',
                   fieldLabel: '銀行代碼',
                   id: 'bank_code',
                   name: 'bank_code',
                   allowBlank: false,
                   maxLength: 7,
                   minLength: 7,
                   regex: /^[0-9]*$/,
                   regexText: "只允許輸入數字！",
                   width: 300

               },
               {
                   xtype: 'textfield',
                   fieldLabel: '銀行名稱',
                   id: 'bank_name',
                   name: 'bank_name',
                   allowBlank: false,
                   width: 300
               }],
        buttons: [
            {
                text: SAVE,
                formBind: true,
                id: 'save',
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        this.disable();
                        form.submit({
                            params: {
                                id: Ext.htmlEncode(Ext.getCmp('id').getValue()),
                                code: Ext.htmlEncode(Ext.getCmp('bank_code').getValue()),
                                name: Ext.htmlEncode(Ext.getCmp('bank_name').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.success) {
                                    Ext.Msg.alert(INFORMATION, "信息保存成功！");
                                    VendorBankStore.load();
                                    editBankWin.close();
                                }
                                else {
                                    if (result.error == "-1") {
                                        Ext.Msg.alert(INFORMATION, "該代碼或名稱已存在,不可重複保存！");
                                    }
                                    else if (result.error != "0") {
                                        Ext.Msg.alert(INFORMATION, result.error);
                                    }
                                    Ext.getCmp('save').setDisabled(false);
                                }
                            },
                            failure: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                if (result.error == "-1") {
                                    Ext.Msg.alert(INFORMATION, "該代碼或名稱已存在,不可重複保存！");
                                }
                                else if (result.error != "0") {
                                    Ext.Msg.alert(INFORMATION, result.error);
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, "信息保存失敗,請聯繫IT人員！");
                                }
                                Ext.getCmp('save').setDisabled(false);
                            }
                        });

                    }
                }
            }
        ]
    });

    var editBankWin = Ext.create('Ext.window.Window', {
        title: "銀行信息編輯",
        id: 'editBankWin',
        iconCls: row ? "icon-user-edit" : "icon-user-add",
        width: 350,
        height: 160,
        layout: 'fit',
        items: [editBankFrm],
        constrain: true, //束縛窗口在框架內
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
            {
                type: 'close',
                qtip: '是否關閉',
                handler: function (event, toolEl, panel) {
                    Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                        if (btn == "yes") {
                            Ext.getCmp('editBankWin').destroy();
                        } else {
                            return false;
                        }
                    });
                }
            }
        ],
        listeners: {
            'show': function () {
                if (row) {
                    editBankFrm.getForm().loadRecord(row);
                }
                else {
                    editBankFrm.getForm().reset();
                }
            }
        }
    });
    editBankWin.show();
}


onAddClick = function () {
    editBankFunction(null);
}

onEditClick = function () {
    var row = Ext.getCmp("gdBank").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editBankFunction(row[0]);
    }
}
