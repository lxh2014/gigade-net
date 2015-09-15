var CallidForm;
var pageSize = 10;
/*吉食分享,本月推薦主頁面*/
//吉食分享,本月推薦Model
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
        { name: "content_title", type: "string" },
        { name: "content_image", type: "string" },
        { name: "home_title", type: "string" },
        { name: "home_text", type: "string" },
        { name: "product_id", type: "int" },
        { name: "product_name", type: "string" },
        { name: "content_default", type: "int" },
        { name: "content_status", type: "int" },
        { name: "link_url", type: "string" },
        { name: "link_page", type: "string" },
        { name: "link_mode", type: "int" },
        { name: "start_time", type: "string" },
        { name: "end_time", type: "string" },
        { name: "update_on", type: "string" },
        { name: "created_on", type: "string" }
    ]
});
//到Controller獲取數據
var WCT2Store = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.Health',
    proxy: {
        type: 'ajax',
        url: '/WebContentType/WebContentTypelist2',
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
WCT2Store.on('beforeload', function () {
    Ext.apply(WCT2Store.proxy.extraParams, {

    });
});
function Query(x) {
    WCT2Store.removeAll();
    Ext.getCmp("gdHealth").store.loadPage(1, {
        params: {
        }
    });
}
Ext.onReady(function () {
    var gdHealth = Ext.create('Ext.grid.Panel', {
        id: 'gdHealth',
        store: WCT2Store,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: CONTENTID, dataIndex: 'content_id', width: 50, align: 'center' },
            { header: SITEID, dataIndex: 'site_name', width: 70, align: 'center' },
            { header: PAGEID, dataIndex: 'page_name', width: 70, align: 'center' },
            { header: AREAID, dataIndex: 'area_name', width: 70, align: 'center' },
            { header: PHOTOTITLE, dataIndex: 'content_title', width: 100, align: 'center' },
        //{ header: CONTENTIMAGE, dataIndex: 'content_image', width: 130, align: 'center' },
            {
                header: CONTENTIMAGE,
                dataIndex: 'content_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<img width=50 name="tplImg" height=50 src="{content_image}" />'
            },
            { header: HOMETITLE, dataIndex: 'home_title', width: 120, align: 'center' },
            {
                header: HOMETEXT, dataIndex: 'home_text', width: 150, align: 'center',
                renderer: function (val) {
                    var xiaohao = new RegExp("<", "g");
                    var dahao = new RegExp(">", "g");
                    val = val.replace(xiaohao, "&lt;").replace(dahao, "&gt;");
                    return val;
                }
            },
            { header: PRODUCTID, dataIndex: 'product_id', width: 70, align: 'center' },
            {
                header: CONTENTDEFAULT, dataIndex: 'content_default', width: 60, align: 'center',
                renderer: function (val) {
                    switch (val) {
                        case 0:
                            return CONTENTDEFAULT;
                            break;
                        case 1:
                            return NOCONTENTDEFAULT;
                            break;
                    }
                }
            },
            { header: LINKURL, dataIndex: 'link_url', width: 130, align: 'center' },
            { header: LINKPAGE, hidden: true, dataIndex: 'link_page', width: 130, align: 'center' },
            {
                header: LINKMODE, dataIndex: 'link_mode', width: 70, align: 'center',
                renderer: function (val) {
                    switch (val) {
                        case 0:
                            return NOLINKMODE;
                            break;
                        case 1:
                            return OLDLINKMODE;
                            break;
                        case 2:
                            return LINKMODE;
                            break;
                    }
                }
            },
             { header: STARTTIME, dataIndex: 'start_time', width: 130, align: 'center' },
            { header: ENDTIME, dataIndex: 'end_time', width: 130, align: 'center' },
            { header: UPDATEON, dataIndex: 'update_on', width: 130, align: 'center' },
            { header: CREATEDON, dataIndex: 'created_on', width: 130, align: 'center' },
            
        //{ header: CONTENTSTATUS, dataIndex: 'content_status', width: 70, align: 'center' },
            {
                header: CONTENTSTATUS, dataIndex: 'content_status', id: 'con_status', align: 'center', // hidden: true,            
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
            { xtype: 'button', text: REMOVE, id: 'remove', hidden: true, iconCls: 'icon-user-remove', disabled: true, handler: onRemoveClick }
        //            {
        //                text: SEARCH,
        //                iconCls: 'icon-search',
        //                id: 'btnQuery',
        //                handler: Query
        //            }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: WCT2Store,
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
    WCT2Store.load({ params: { start: 0, limit: 10 } });
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
    editFunction(null, WCT2Store);
}
/*編輯*/
onEditClick = function () {
    var row = Ext.getCmp("gdHealth").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], WCT2Store);
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
                    url: '/WebContentType/WebContentTypeDelete',
                    method: 'post',
                    params: { rowID: rowIDs },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        Ext.Msg.alert(INFORMATION, SUCCESS);
                        if (result.success) {
                            WCT2Store.load(1);
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
/*控件權限設定*/
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
/*人員設定*/
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
//更新是否可用狀態
function UpdateActive(id, pageId, areaId) {
    var activeValue = $("#img" + id).attr("hidValue"); //hidValue=1時是將要變成啟用
    var limitN = 0;
    var listN = 0;
    Ext.Ajax.request({
        url: "/WebContentType/GetDefaultLimit",
        method: 'post',
        async: false, //true為異步，false為異步
        params: {
            storeType: "web_content_type2",
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

        Ext.Msg.confirm(CONFIRM, Ext.String.format(STATUSTIP), function (btn) {
            if (btn == 'yes') {
                Ext.Ajax.request({
                    url: "/WebContentType/UpdateActive",
                    method: 'post',
                    params: {
                        id: id,
                        active: activeValue,
                        storeType: "type2"
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            WCT2Store.load();
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
            params: {
                id: id,
                active: activeValue,
                storeType: "type2"
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    WCT2Store.load();
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