var CallidForm;
var pageSize = 10;
/*明星,達人管理主頁面*/
//明星,達人管理Model
Ext.define('gigade.Health', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "content_id", type: "int" },
        { name: "site_id", type: "int" },
        { name: "site_name", type: "string" },
        { name: "page_id", type: "int" },
        { name: "page_name", type: "string" },
        { name: "area_id", type: "int" },
        { name: "area_name", type: "string" },
        { name: "type_id", type: "int" },
        { name: "home_title", type: "string" },
        { name: "home_text", type: "string" },
        { name: "content_title", type: "string" },
        { name: "content_html", type: "string" },
        { name: "content_image", type: "string" },
        { name: "content_default", type: "int" },
        { name: "content_status", type: "int" },
        { name: "link_url", type: "string" },
        { name: "link_mode", type: "int" },
        { name: "update_on", type: "string" },
        { name: "created_on", type: "string" },
        { name: "keywords", type: "string" },
        { name: "sort", type: "string" }, //排序
        { name: "start_time", type: "string" },
        { name: "end_time", type: "string" }
        
    ]
});

var serchPageidStore = Ext.create('Ext.data.Store', {
    fields: ['serch_name', 'serch_id'],
    data: [
        { "serch_name": "不分", "serch_id": 0 },
        { "serch_name": "明星食物", "serch_id": 5 },
        { "serch_name": "達人專區", "serch_id": 6 }
    ]
});

//到Controller獲取數據
var HealthStore = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.Health',
    proxy: {
        type: 'ajax',
        url: '/WebContentType/WebContentTypelist7',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'//總行數
        }
    }
});
//勾選框
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdHealth").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdHealth").down('#remove').setDisabled(selections.length == 0);
        }
    }
});
HealthStore.on('beforeload', function () {
    Ext.apply(HealthStore.proxy.extraParams, {
        serchcontent: Ext.getCmp('serchcontent').getValue(),
        serchchoose: Ext.getCmp('serch').getValue()
    });
});
function Query(x) {
    HealthStore.removeAll();
    Ext.getCmp("gdHealth").store.loadPage(1, {
        params: {
            serchcontent: Ext.getCmp('serchcontent').getValue(),
            serchchoose: Ext.getCmp('serch').getValue()
        }
    });
}
Ext.onReady(function () {
  
    var gdHealth = Ext.create('Ext.grid.Panel', {
        id: 'gdHealth',
        store: HealthStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: CONTENTID, dataIndex: 'content_id', width: 50, align: 'center' },
            { header: SITEID, dataIndex: 'site_name', width: 70, align: 'center' },
            { header: PAGEID, dataIndex: 'page_name', width: 70, align: 'center' },
            { header: AREAID, dataIndex: 'area_name', width: 70, align: 'center' },
            { header: TYPRID, dataIndex: 'type_id', width: 100, align: 'center', hidden: true },
            { header: HOMETITLE, dataIndex: 'home_title', width: 130, align: 'center' },
            { header: HOMETEXT, dataIndex: 'home_text', width: 130, align: 'center',
                renderer: function (val) {
                    var xiaohao = new RegExp("<", "g");
                    var dahao = new RegExp(">", "g");
                    val = val.replace(xiaohao, "&lt;").replace(dahao, "&gt;");
                    return val;
                }
            },
            { header: CONTENTTITLE, dataIndex: 'content_title', width: 130, align: 'center' },
            {
                header: CONTENTHTML,
                dataIndex: 'content_html',
                width: 130,
                align: 'center',
                renderer: function (val) {
                    var xiaohao = new RegExp("<", "g");
                    var dahao = new RegExp(">", "g");
                    val = val.replace(xiaohao, "&lt;").replace(dahao, "&gt;");
                    return val;
                }
            },
            {
                header: CONTENTIMAGE,
                dataIndex: 'content_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<img width=50 name="tplImg" height=50 src="{content_image}" />'
            },
            {
                header: CONTENTDEFAULT,
                dataIndex: 'content_default',
                width: 60,
                align: 'center',
                renderer: function (val) {
                    switch (val) {
                        case 0:
                            return "預設";
                            break;
                        case 1:
                            return "非預設";
                            break;
                    }
                }
            },
            { header: LINKURL, dataIndex: 'link_url', width: 130, align: 'center' },
            {
                header: LINKMODE,
                dataIndex: 'link_mode',
                width: 70,
                align: 'center',
                renderer: function (val) {
                    switch (val) {
                        case 0:
                            return "不開";
                            break;
                        case 1:
                            return "開原視窗";
                            break;
                        case 2:
                            return "開新視窗";
                            break;
                    }
                }
            },
            { header: UPDATEON, dataIndex: 'update_on', width: 130, align: 'center' },
            { header: CREATEDON, dataIndex: 'created_on', width: 130, align: 'center' },
            { header: "排序", dataIndex: 'sort', width: 100, align: 'center' },
            { header: "上架時間", dataIndex: 'start_time', width: 130, align: 'center' },
            { header: "下架時間", dataIndex: 'end_time', width: 130, align: 'center' },
             { header: CONTENTSTATUS,
                 dataIndex: 'content_status',
                 id: 'con_status',
                 // hidden: true,
                 align: 'center',
                 renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value == 1) {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.content_id + "," + record.data.page_id + "," + record.data.area_id + ")'><img hidValue='0' id='img" + record.data.content_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                     } else {
                         return "<a href='javascript:void(0);' onclick='UpdateActive(" + record.data.content_id + "," + record.data.page_id + "," + record.data.area_id + ")'><img hidValue='1' id='img" + record.data.content_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                     }
                 }
             }
        ],
        tbar: [
            { xtype: 'button', text: ADD, id: 'add', hidden: false, iconCls: 'icon-user-add', handler: onAddClick },
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick },
             '->',
             { xtype: 'combobox', fieldLabel: "查詢條件", labelWidth: 55, id: 'serch', emptyText: "請選擇", store: serchPageidStore, displayField: 'serch_name', valueField: 'serch_id' },
             { xtype: 'textfield', fieldLabel: "查詢內容", id: 'serchcontent', labelWidth: 55 },
            {
                text: "搜索",
                iconCls: 'icon-search',
                id: 'btnQuery',
                hidden: false,
                handler: Query
            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: HealthStore,
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
        items: [gdHealth],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdHealth.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    HealthStore.load({ params: { start: 0, limit: 10} });
});

function imgFadeBig(img, width, height) {
    var e = this.event;
    if (img.split('/').length != 5) {
        $("#imgTip").attr("src", img)
            .css({
                "top": (e.clientY < height ? e.clientY : e.clientY - height) + "px",
                "left": (e.clientX) + "px",
                "width": width + "px",
                "height": height + "px"
            }).show();
    }
}
/*新增*/
onAddClick = function () {
    editFunction(null, HealthStore);
}
/*編輯*/
onEditClick = function () {
    var row = Ext.getCmp("gdHealth").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], HealthStore);
    }
}
/*刪除*/
onRemoveClick = function () {
    var row = Ext.getCmp("gdHealth").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var rowIDs = '';
                for (var i = 0; i < row.length; i++) {
                    rowIDs += row[i].data.id + '|';
                }
                Ext.Ajax.request({
                    url: '/WebContentType/DeleteWebContentType7',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            HealthStore.load(1);
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
/*权限分配*/
onAuthClick = function () {
    var row = Ext.getCmp("gdHealth").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        groupAuthority(row[0].data.rowid);
    }
}
/*人員管理*/
onCallidClick = function () {
    var row = Ext.getCmp("gdHealth").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        var groupId = Ext.getCmp('gdHealth').getSelectionModel().getSelection()[0].data.rowid;
        Ext.Ajax.request({
            url: '/Fgroup/QueryCallidById',
            params: { groupId: groupId },
            success: function (response) {
                var a = response.responseText;
                var arr = a.split(",");
                if (!CallidForm) {
                    CallidForm = Ext.create('widget.window', {
                        title: TOOL_CALLID, closable: true,
                        closeAction: 'hide',
                        modal: true,
                        width: 500,
                        minWidth: 500,
                        height: document.documentElement.clientHeight * 300 / 783,
                        layout: 'fit',
                        bodyStyle: 'padding:5px;',
                        items: [{
                            xtype: 'itemselector',
                            name: 'itemselector',
                            id: 'itemselector-field',
                            anchor: '100%',
                            store: ManageUserStore,
                            displayField: 'name',
                            valueField: 'callid',
                            allowBlank: false,
                            msgTarget: 'side'
                        }, {
                            xtype: 'textfield',
                            name: 'groupId',
                            hidden: true
                        }],
                        fbar: [{
                            xtype: 'button',
                            text: RESET,
                            id: 'reset',
                            handler: function () {
                                Ext.getCmp("itemselector-field").reset();
                                return false;
                            }
                        },
                        {
                            xtype: 'button',
                            text: SAVE,
                            id: 'save',
                            handler: function () {
                                Ext.Ajax.request({
                                    url: '/Fgroup/AddCallid',
                                    params: { groupId: Ext.getCmp('gdHealth').getSelectionModel().getSelection()[0].data.rowid, callid: Ext.getCmp("itemselector-field").getValue() },
                                    success: function (response, opts) {
                                        var result = eval("(" + response.responseText + ")");
                                        Ext.Msg.alert(INFORMATION, SUCCESS);
                                        CallidForm.hide();
                                    },
                                    failure: function (response) {
                                        var result = eval("(" + response.responseText + ")");
                                        Ext.Msg.alert(INFORMATION, FAILURE);
                                    }
                                });
                            }
                        }]
                    });
                }

                CallidForm.show();
                Ext.getCmp("itemselector-field").setValue(arr);
            },
            failure: function (response) {
                var resText = eval("(" + response.responseText + ")");
                alert(resText.rpackCode);
            }
        });
    }
}
//更改活動狀態(設置活動可用與不可用)
function UpdateActive(id, pageId, areaId) {
    var activeValue = $("#img" + id).attr("hidValue"); //hidValue=1時是將要變成啟用
    var limitN = 0;
    var listN = 0;
    Ext.Ajax.request({
        url: "/WebContentType/GetDefaultLimit",
        method: 'post',
        async: false, //true為異步，false為異步
        params: {
            storeType: "web_content_type7",
            site: "7",
            page: pageId,
            area: areaId
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                listN = result.listNum;
                limitN = result.limitNum;
            }
        }
    });
    if (activeValue == "1" && listN >= limitN && limitN != 0) {//list的值大於或等於limit的值時提示信息，yes時執行，no時返回

        Ext.Msg.confirm(CONFIRM, Ext.String.format("啟用數目已達極限，將取消最舊的一條，是否執行？"), function (btn) {
            if (btn == 'yes') {
                Ext.Ajax.request({
                    url: "/WebContentType/UpdateActive",
                    method: 'post',
                    params: { id: id,
                        active: activeValue,
                        storeType: "type7"
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            HealthStore.load();
                            if (activeValue == 1) {
                                $("#img" + id).attr("hidValue", 0);
                                $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                            } else {
                                $("#img" + id).attr("hidValue", 1);
                                $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                            }
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, FAILURE);
                        }

                    },
                    failure: function () {
                        Ext.Msg.alert(INFORMATION, FAILURE);
                    }
                });
            }
            else { return; }
        });
    }
    else {
        Ext.Ajax.request({
            url: "/WebContentType/UpdateActive",
            method: 'post',
            params: { id: id,
                active: activeValue,
                storeType: "type7"
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    HealthStore.load();
                    if (activeValue == 1) {
                        $("#img" + id).attr("hidValue", 0);
                        $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                    } else {
                        $("#img" + id).attr("hidValue", 1);
                        $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                    }
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }

            },
            failure: function () {
                Ext.Msg.alert(INFORMATION, FAILURE);
            }
        });
    }
}

