var CallidForm;
var pageSize = 25;

//料位管理Model
Ext.define('gigade.Ilocs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "row_id", type: "int" },
        { name: "item_id", type: "int" },      //商品品號
        { name: "product_name", type: "string" },   //品名
        { name: "prod_qty", type: "string" },       //數量
        { name: "plas_loc_id", type: "string" },    //上架料位
        { name: "cde_dt", type: "string" },         //有效日期
        { name: "ista_id", type: "string" },
        { name: "loc_id", type: "string" },         //料位編號
        { name: "cde_dt_var", type: "int" },  //允收天數
        { name: "qity_name", type: "string" },
        { name: "create_dtim", type: "string" },
        { name: "user_name", type: "string" }        
    ]
});

var IinvdStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ilocs',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIinvdList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }

});
var SearchStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
        { "txt": "料位", "value": "1" },
        { "txt": "條碼", "value": "2" },
        { "txt": "商品品號", "value": "3" }
    ]
});
IinvdStore.on('beforeload', function () {
    Ext.apply(IinvdStore.proxy.extraParams, {
        starttime: Ext.getCmp('start_time').getValue(),
        endtime: Ext.getCmp('end_time').getValue(),
        search_type: Ext.getCmp('Search_type').getValue(),
        searchcontent: Ext.getCmp('searchcontent').getValue()
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
           // Ext.getCmp("gdIinvd").down('#edit').setDisabled(selections.length == 0);
        }
    }
});

function Query(x) {
    IinvdStore.removeAll();
    var start = Ext.getCmp("start_time");
    var end = Ext.getCmp("end_time");
    var search = Ext.getCmp("searchcontent");
    if (search.getValue().trim() =="") {
        
        if (start.getValue() == null||end.getValue() == null) {
            Ext.Msg.alert("提示", "請輸入查詢時間或查詢內容");
            return;
        }
    }
    Ext.getCmp("gdIinvd").store.loadPage(1, {
        params: {
            starttime: Ext.getCmp('start_time').getValue(),
            endtime: Ext.getCmp('end_time').getValue(),
            search_type: Ext.getCmp('Search_type').getValue(),
            searchcontent: Ext.getCmp('searchcontent').getValue().trim()
        }
    });
}

Ext.onReady(function () {

    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        height: 80,
        border: 0,
        bodyPadding: 10,
        width: document.documentElement.clientWidth,
        items: [
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                        {
                            xtype: 'combobox',
                            name: 'Search_type',
                            id: 'Search_type',
                            editable: false,
                            fieldLabel: "查詢條件",
                            labelWidth: 60,
                            margin: '0 5 0 0',
                            store: SearchStore,
                            queryMode: 'local',
                            submitValue: true,
                            displayField: 'txt',
                            valueField: 'value',
                            typeAhead: true,
                            forceSelection: false,
                            value: 2
                        },
                        { xtype: 'textfield', allowBlank: true, id: 'searchcontent', name: 'searchcontent' },
                        { xtype: 'label', margin: '2 0 0 10', text: '創建時間:' },
                        {
                            xtype: "datefield",
                            editable: false,
                            margin: '0 0 0 5',
                            id: 'start_time',
                            name: 'start_time',
                            format: 'Y/m/d',
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("start_time");
                                    var end = Ext.getCmp("end_time");
                                    if (end.getValue() == null) {
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    } else if (start.getValue() > end.getValue()) {
                                        Ext.Msg.alert(INFORMATION, "開始時間不能大於結束時間");
                                        end.setValue(setNextMonth(start.getValue(), 1));
                                    }
                                    //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                    //    // Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                    //    end.setValue(setNextMonth(start.getValue(), 1));
                                    //}
                                },
                                specialkey: function (field, e) {
                                    if (e.getKey() == e.ENTER) {
                                        Query();
                                    }
                                }
                            }
                        },
                        { xtype: 'displayfield', value: '~ ' },
                        {
                            xtype: "datefield",
                            editable: false,
                            id: 'end_time',
                            name: 'end_time',
                            format: 'Y/m/d',
                            listeners: {
                                select: function (a, b, c) {
                                    var start = Ext.getCmp("start_time");
                                    var end = Ext.getCmp("end_time");
                                    var s_date = new Date(start.getValue());
                                    var now_date = new Date(end.getValue());
                                    if (start.getValue() != "" && start.getValue() != null) {
                                        if (end.getValue() < start.getValue()) {
                                            Ext.Msg.alert(INFORMATION, "結束時間不能小於開始時間");
                                            end.setValue(setNextMonth(start.getValue(), 1));
                                        }
                                        //else if (end.getValue() > setNextMonth(start.getValue(), 1)) {
                                        //    //Ext.Msg.alert(INFORMATION, DATE_LIMIT);
                                        //    start.setValue(setNextMonth(end.getValue(), -1));
                                        //}

                                    } else {
                                        start.setValue(setNextMonth(end.getValue(), -1));
                                    }
                                },
                                specialkey: function (field, e) {
                                    if (e.getKey() == e.ENTER) {
                                        Query();
                                    }
                                }
                            }

                        }
                 ]
             },
             {
                 xtype: 'fieldcontainer',
                 combineErrors: true,
                 layout: 'hbox',
                 items: [
                        {
                            xtype: 'button',
                            text: SEARCH,
                            iconCls: 'icon-search',
                            id: 'btnQuery',
                            handler: Query
                        },
                        {
                            xtype: 'button',
                            text: RESET,
                            id: 'btn_reset',
                            margin: '0 0 0 10',
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners: {
                                click: function () {
                                    Ext.getCmp("searchcontent").setValue("");
                                    Ext.getCmp('start_time').setValue("");
                                    Ext.getCmp('end_time').setValue("");
                                    Ext.getCmp('Search_type').setValue(2);
                                }
                            }
                        }
                 ]
             }
        ]
    });
    var gdIinvd = Ext.create('Ext.grid.Panel', {
        id: 'gdIinvd',
        store: IinvdStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 9.4,
        columns: [
            { header: "編號", dataIndex: 'row_id', width: 100, align: 'center' },
            { header: "商品品號", dataIndex: 'item_id', width: 100, align: 'center' },
            { header: "品名", dataIndex: 'product_name', width: 150, align: 'center' },
            { header: "數量", dataIndex: 'prod_qty', width: 100, align: 'center' },
            { header: "有效日期", dataIndex: 'cde_dt', width: 150, align: 'center' },
            { header: "上架料位", dataIndex: 'plas_loc_id', width: 100, align: 'center' },
            { header: "主料位", dataIndex: 'loc_id', width: 100, align: 'center' },
            { header: "允收天數", dataIndex: 'cde_dt_var', width: 100, align: 'center' },
            { header: "建立日期", dataIndex: 'create_dtim', width: 100, align: 'center' },
            { header: "建立人員", dataIndex: 'user_name', width: 100, align: 'center' },
            {
                header: "鎖定", dataIndex: 'ista_id', width: 100, align: 'center',//H鎖定
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "H") {//H鎖定的
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='H' id='img" + record.data.row_id + "' src='../../../Content/img/icons/hmenu-lock.png'/></a>";
                    } else if (value == "A") {
                        return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.row_id + ")'><img hidValue='A' id='img" + record.data.row_id + "' src='../../../Content/img/icons/hmenu-unlock.png'/></a>";
                    }
                }
            },
            {
                header: "原因", dataIndex: 'qity_name', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value == "" || value == null) {//H鎖定的
                        return "正常";
                    }
                    else {
                        return value;
                    }
                }
            }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            //{ xtype: 'button', text: '變更庫存', id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: "匯出Excel", id: 'ExportOut', icon: '../../../Content/img/icons/excel.gif', handler: onExportOut },
          
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: IinvdStore,
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
        }
        ,//多选框注释
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm,gdIinvd],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdIinvd.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

Ext.define("gigade.lock", {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ParameterCode", type: "int" },
        { name: "parameterName", type: "string" }]
});

var LockStore = Ext.create('Ext.data.Store', {
    model: 'gigade.lock',
    //autoLoad: true,
    proxy: {
        type: 'ajax',
        url: "/WareHouse/GetWhyLock",
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'data'
        }

    }
});


/************************新增************************************/
onAddClick = function () {
    //addWin.show();
    addFunction(null, IinvdStore);
}
/*********************************************編輯***************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdIinvd").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        UpdFunction(row[0], IinvdStore);
    }
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    } else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
function UpdateActive(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    if (activeValue == "A")
    {
        var qitylock = Ext.create('Ext.form.Panel', {
            id: 'qitylock',
            frame: true,
            plain: true,
            layout: 'anchor',
            labelWidth: 45,
            url: '/WareHouse/UpdateIinvdActive',
            defaults: { anchor: "95%", msgTarget: "side" },
            items: [
                {
                    xtype: 'combobox',
                    fieldLabel: '鎖定原因',
                    id: 'lock_id',
                    name: 'lock_id',
                    allowBlank: false,
                    editable: false,
                    typeAhead: true,
                    forceSelection: false,
                    store: LockStore,
                    displayField: 'parameterName',
                    valueField: 'ParameterCode',
                    emptyText:"請選擇"
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '相關單號',
                    id: 'po_id',
                    colName: 'po_id',
                    submitValue: false,
                    name: 'po_id'
                },
                {
                    xtype: 'textfield',
                    fieldLabel: '備註原因',
                    id: 'remarks',
                    colName: 'remarks',
                    submitValue: false,
                    name: 'remarks'
                }
            ],
            buttons: [{
                text: SAVE,
                formBind: true,
                disabled: true,
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        form.submit({
                            params: {
                                lock_id: Ext.htmlEncode(Ext.getCmp('lock_id').getValue()),
                                "id": id,
                                "active": activeValue,
                                "po_id": Ext.htmlEncode(Ext.getCmp('po_id').getValue()),
                                "remarks": Ext.htmlEncode(Ext.getCmp('remarks').getValue())
                            },
                            success: function (form, action) {
                                var result = Ext.decode(action.response.responseText);
                                Ext.Msg.alert(INFORMATION, SUCCESS);
                                if (result.success) {
                                    IinvdStore.load();
                                    whylock.close();
                                } else {
                                    Ext.MessageBox.alert(ERRORSHOW + result.success);
                                }
                            },
                            failure: function () {
                                Ext.Msg.alert(INFORMATION, FAILURE);
                            }
                        });
                    }
                }
            }]
        });
        var whylock = Ext.create('Ext.window.Window', {
            title: "鎖定原因",
            id: 'whylock',
            iconCls: 'icon-user-edit',
            width: 350,
            y: 100,
            layout: 'fit',
            items: [qitylock],
            closeAction: 'destroy',
            modal: true,
            constrain: true,    //窗體束縛在父窗口中
            resizable: false,
            labelWidth: 60,
            bodyStyle: 'padding:5px 5px 5px 5px',
            closable: false,
            tools: [
             {
                 type: 'close',
                 qtip: CLOSEFORM,
                 handler: function (event, toolEl, panel) {
                     Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                         if (btn == "yes") {
                             Ext.getCmp('whylock').destroy();
                         }
                         else {
                             return false;
                         }
                     });
                 }
             }]
        }).show();
    }
    else
    {
        var codeFrmlist = Ext.create('Ext.form.Panel', {
            id: 'codeFrmlist',
            frame: true,
            plain: true,
            constrain: true,
            defaultType: 'textfield',
            autoScroll: true,
            layout: 'anchor',
            // url: '/WareHouse/SaveIupc',`
            defaults: { anchor: "95%", msgTarget: "side" },
            items: [
                {
                    xtype: 'displayfield',
                    id: 'info',
                    name: 'info',
                    value: '是否確定要解鎖?',
                    submitValue: true
                },
                {
                    xtype: 'fieldcontainer',
                    combineErrors: true,
                    layout: 'hbox',
                    items: [
                        {
                            xtype: 'textfield',
                            fieldLabel: '驗證碼',
                            id: 'check_code',
                            name: 'check_code',
                            margin: '5 7 0 5',
                            allowBlank: false,
                            labelWidth: 45,
                            width: 150,
                            submitValue: true
                        },
                       {
                           xtype: 'displayfield',
                           name: 'showcode',
                           id: 'showcode',
                           margin: '5 3 0 1',
                           submitValue: true,
                           value: returnCode()
                       },
                    ]
                }
            ],
            //buttonAlign: 'right',
            buttons: [
                {
                formBind: true,
                disabled: true,
                text: '確定',
                handler: function () {
                    var form = this.up('form').getForm();
                    if (form.isValid()) {
                        var showcode = Ext.getCmp('showcode').getValue();
                        var checkcode = Ext.getCmp('check_code').getValue();
                        if (checkcode == showcode) {
                            codeWin.close();

                            Ext.Ajax.request({
                                url: '/WareHouse/UpdateIinvdActive',//執行方法
                                method: 'post',
                                params: {
                                    "id": id,
                                    "active": activeValue,
                                    po_id: "",
                                    remarks:""
                                },
                                success: function (form, action) {
                                    var result = Ext.decode(form.responseText);
                                    if (result.success) {
                                        Ext.Msg.alert(INFORMATION, "更改成功!");
                                     
                                        IinvdStore.load();
                                    }
                                },
                                failure: function () {
                                    Ext.Msg.alert(INFORMATION, FAILURE);
                                }
                            });

                            //$.ajax({
                            //    url: "/WareHouse/UpdateIinvdActive",
                            //    data: {
                            //        "id": id,
                            //        "active": activeValue
                            //    },
                            //    type: "POST",
                            //    dataType: "json",
                            //    success: function (msg) {
                            //        Ext.Msg.alert(INFORMATION, SUCCESS);
                            //        IinvdStore.load();
                            //    },
                            //    error: function (msg) {
                            //        Ext.Msg.alert("提示信息", "更改失敗!");
                            //    }
                            //});


                        }
                        else {
                            return false;
                        }
                    }
                }
            }]
        });
        var codeWin = Ext.create('Ext.window.Window', {
            iconCls: 'icon-user-edit',
            id: 'codeWin',
            width: 300,
            // height:300,
            y: 100,
            layout: 'fit',
            items: [codeFrmlist],
            constrain: true,
            closeAction: 'destroy',
            modal: true,
            resizable: false,
            bodyStyle: 'padding:5px 5px 5px 5px',
            closable: false


        });
        codeWin.show();
     
    }
}
function UpdFunction(row, store) {
    var ID, qty;
    if (row != null) {
        ID = row.data["row_id"];
        qty = row.data["prod_qty"];
    }
    var UpdFrm = Ext.create('Ext.form.Panel', {
        id: 'UpdFrm',
        frame: true,
        plain: true,
        defaultType: 'textfield',
        layout: 'anchor',
        labelWidth: 120,
        url: '/WareHouse/UpdProdqty',
        defaults: { anchor: "95%", msgTarget: "side" },
        items: [
            {
                xtype: 'numberfield',
                fieldLabel: "變更庫存數量",
                name: 'change_num',
                id: 'change_num',
                allowBlank: false
            }
        ],
        buttons: [
        {
            text: SAVE,
            formBind: true,
            handler: function () {
                var form = this.up('form').getForm();
                var change_num = Ext.getCmp('change_num').getValue();
                if (form.isValid()) {
                    form.submit({
                        params: {
                            row_id: ID,//編號
                            change_num: change_num,//應加數量
                            from_num: qty//原有數量
                        },
                        success: function (form, action) {
                            var result = Ext.decode(action.response.responseText);
                            if (result.success) {
                                if (result.msg == "1") {
                                    Ext.Msg.alert(INFORMATION, "您要減少的庫存過多!");
                                }
                                else {
                                    Ext.Msg.alert(INFORMATION, SUCCESS);
                                    IinvdStore.load();
                                    editWin.close();
                                }
                            }
                        },
                        failure: function (form, action) {
                            alert(111);
                        }
                    });
                }
            }

        }]
    });

    var editWin = Ext.create('Ext.window.Window', {
        title: "變更庫存",
        id: 'myeditWin',
        iconCls: 'icon-user-edit',
        width: 400,
        height: 200,
        autoScroll: true,
        //        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [UpdFrm],
        closeAction: 'destroy',
        modal: true,
        constrain: true,    //窗體束縛在父窗口中
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        closable: false,
        tools: [
         {
             type: 'close',
             qtip: CLOSEFORM,
             handler: function (event, toolEl, panel) {
                 Ext.MessageBox.confirm(CONFIRM, IS_CLOSEFORM, function (btn) {
                     if (btn == "yes") {
                         Ext.getCmp('myeditWin').destroy();
                     }
                     else {
                         return false;
                     }
                 });
             }
         }]
    });
    editWin.show();
}

onExportOut = function () {
    window.open("/WareHouse/IinvdExcelList?searchcontent=" + Ext.getCmp('searchcontent').getValue() + "&search_type=" + Ext.getCmp('Search_type').getValue() + "&starttime=" + Ext.Date.format(new Date(Ext.getCmp('start_time').getValue()), 'Y-m-d H:i:s') + "&endtime=" + Ext.Date.format(new Date(Ext.getCmp('end_time').getValue()), 'Y-m-d H:i:s'));
}

function Tomorrow() {
    var d;
    d = new Date();
    d.setDate(d.getDate() + 1);
    return d;
}