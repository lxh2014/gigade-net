var pageSize = 25;
var subscribe = document.getElementById('subscribe').value;
Ext.define('gigade.EdmContentNew', {
    extend: 'Ext.data.Model',
    fields: [
    { name: "content_id", type: "int" },
    { name: "group_id", type: "int" },
    { name: "subject", type: "string" },
    { name: "template_id", type: "int" },
    { name: "template_data", type: "string" },
    { name: "template_data_send", type: "string" },
    { name: "importance", type: "int" },
    { name: "sender_id", type: "int" },
    { name: "content_createdate", type: "string" },
    { name: "content_updatedate", type: "string" },
    { name: "content_create_userid", type: "int" },
    { name: "content_update_userid", type: "int" },
    { name: "schedule_date", type: "string" },
    { name: "count", type: "int" },
    { name: "date", type: "string" },
    { name: "sender_email", type: "string" },
    { name: "sender_name", type: "string" },
    { name: "edit_url", type: "string" },
   { name: "content_url", type: "string" },
   { name: "pm", type: "int" },
   { name: "edm_pm", type: "string" },
   { name: "static_template", type: "int" },
      { name: "user_username_create", type: "string" },
           { name: "user_username_update", type: "string" },
   
    ]
});
EdmContentNewStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.EdmContentNew',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/GetECNList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
Ext.define('gigade.edm_group_new2', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'group_id', type: 'int' },
        { name: 'group_name', type: 'string' }
    ]
});
var EdmGroupNewStore2 = Ext.create("Ext.data.Store", {
    autoLoad: true,
    model: 'gigade.edm_group_new2',
    proxy: {
        type: 'ajax',
        url: '/EdmNew/GetEdmGroupNewStore',
        reader: {
            type: 'json',
            root: 'data'
        }
    }
});
EdmContentNewStore.on('beforeload', function () {
    Ext.apply(EdmContentNewStore.proxy.extraParams,
    {
        group_id: Ext.getCmp('search_group_name').getValue(),
    });
});

var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("EdmContentNew").down('#edit').setDisabled(selections.length == 0);
            //Ext.getCmp("EdmContentNew").down('#report').setDisabled(selections.length == 0);
            Ext.getCmp("EdmContentNew").down('#goSend').setDisabled(selections.length == 0);


        }
    }
});

Ext.onReady(function () {
    var EdmContentNew = Ext.create('Ext.grid.Panel', {
        id: 'EdmContentNew',
        store: EdmContentNewStore,
        flex: '8.9',
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
        { header: "編號", dataIndex: 'content_id', width: 60, align: 'center' },
          { header: "需求申請者", dataIndex: 'edm_pm', width: 85, align: 'center' },
        { header: "正式發送", dataIndex: 'count', width: 150, align: 'center' },
        { header: "郵件主旨", dataIndex: 'subject', width: 200, align: 'center' },
                {
                    header: "報表", width: 100, align: 'center', hidden: false, id: 'reportEdmContentNew',
                    renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                        return "<a href='javascript:void(0)' onclick='ContentNewReportList(" + record.data.content_id + ")'><img src='../../../Content/img/icon_report.gif' /></a>"
                    }
                },
        {
            header: "發送時間", dataIndex: 'date', width: 150, align: 'center'
            , renderer: function (value) {
                if (value == "0001-01-01 00:00:00") {
                    return "";
                }
                else {
                    return value;
                }
            }
        },
        {
            header: "預覽電子報", width: 100, align: 'center', hidden: false, id: 'reviewEdm',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<a href='javascript:void(0)' onclick='ReviewEdm()'><img src='../../../Content/img/icon_report.gif' /></a>"
            }
        },
        { header: "建立人", dataIndex: 'user_username_create', width: 85, align: 'center' },
           { header: "更新人", dataIndex: 'user_username_update', width: 85, align: 'center' },
        ],
        tbar: [
        { xtype: 'button', text: '新增', id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
        { xtype: 'button', text: '編輯', id: 'edit', hidden: false, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
        { xtype: 'button', text: "前往發送", id: 'goSend', hidden: false, disabled: true, handler: onGoSendClick },
         '->',
         {
             xtype: 'combobox', fieldLabel: '電子報類型', id: 'search_group_name', store: EdmGroupNewStore2, displayField: 'group_name',
             valueField: 'group_id', editable: false, value: 0, lastQuery: '', emptyText: '全部',
         },
         {
             xtype: 'button', text: '查詢', handler: Search
         },
         {
             xtype: 'button', text: '重置', handler: function () {
                 Ext.getCmp('search_group_name').setValue();
             }
         },
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EdmContentNewStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: "當前顯示記錄" + ': {0} - {1}' + "共計" + ': {2}',
            emptyMsg: "沒有記錄可以顯示"
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
        layout: 'vbox',
        items: [EdmContentNew],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                EdmContentNew.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
});

/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    editFunction(null, EdmContentNewStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行");
    } else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行");
    } else if (row.length == 1) {
        editFunction(row[0], EdmContentNewStore);
    }
}

onGoSendClick = function () {
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert("提示信息", "沒有選擇一行");
    } else if (row.length > 1) {
        Ext.Msg.alert("提示信息", "只能選擇一行");
    } else if (row.length == 1) {
           row[0].data.template_data_send = row[0].data.template_data;
            sendFunction(row[0], EdmContentNewStore);
       
 

    }
}


onStatusClick = function () {
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    statusFunction(row[0], EdmContentNewStore);
}


function Search() {
    EdmContentNewStore.removeAll();
    var group_id = Ext.getCmp('search_group_name').getValue();
    if (group_id != 0 && group_id != null) {
        Ext.getCmp("EdmContentNew").store.loadPage(1, {
            params: {
                group_id: group_id,
            }
        });
    } else {
        Ext.Msg.alert("提示信息","請選擇查詢條件");
        return;
    }

}

function ContentNewReportList(content_id) {
    var urlTran = '/EdmNew/EdmContentNewReport?content_id=' + content_id;
    var panel = window.parent.parent.Ext.getCmp('ContentPanel');
    var copy = panel.down('#EdmContentNew');
    if (copy) {
        copy.close();
    }
    copy = panel.add({
        id: 'EdmContentNew',
        title: '電子報統計報表',
        html: window.top.rtnFrame(urlTran),
        closable: true
    });
    panel.setActiveTab(copy);
    panel.doLayout();
}

function ReviewEdm() {
  
    var checked;
    var template_data;
    var row = Ext.getCmp("EdmContentNew").getSelectionModel().getSelection();
    var static_template=row[0].data.static_template;
    if (row[0].data.template_data.indexOf(subscribe) > 0) {
        checked = true;
        template_data = row[0].data.template_data.replace(subscribe, "");
    }
    else {
        checked = false;
        template_data = row[0].data.template_data;
    }
    var myMask = new Ext.LoadMask(Ext.getBody(), { msg: "生成預覽中..." });
    myMask.show();
    Ext.Ajax.request({
        url: '/EdmNew/GetPreviewHtml',
        params: {
            content_id: row[0].data.content_id,
            template_data: template_data,
            checked: checked,
            static_template: static_template,
          
        },
        success: function (data) {
           myMask.hide();
            var result = data.responseText;
            var A = 1000;
            var B = 700;
            var C = (document.body.clientWidth - A) / 2;
            var D = window.open('', null, 'toolbar=yes,location=no,status=yes,menubar=yes,scrollbars=yes,resizable=yes,width=' + A + ',height=' + B + ',left=' + C);
            var E = "<html><head><title>預覽</title></head><style>body{line-height:200%;padding:50px;}</style><body><div >" + result + "</div></body></html>";
            D.document.write(E);
            D.document.close();
        }
    });

}


