var CallidForm;
var pageSize = 10;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.WCT8', {
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
        { name: "big_title", type: "string" },    //大標
        { name: "small_title", type: "string" },  //小標
        { name: "home_image", type: "string" },
        { name: "content_default", type: "int" },
        { name: "content_status", type: "int" },
        { name: "link_url", type: "string" },
        { name: "link_mode", type: "int" },
        { name: "update_on", type: "string" },
        { name: "created_on", type: "string" },
        { name: "sort", type: "int" },          //排序
        { name: "start_time", type: "string" }, //上架時間
        { name: "end_time", type: "string" }    //下架時間
    ]
});
//到Controller獲取數據
var WCT8Store = Ext.create('Ext.data.Store', {
    autoDestroy: true, //自動消除
    pageSize: pageSize,
    model: 'gigade.WCT8',
    proxy: {
        type: 'ajax',
        url: '/WebContentType/WebContentTypelist8',
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
            Ext.getCmp("gdFgroup").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
WCT8Store.on('beforeload', function () {
    Ext.apply(WCT8Store.proxy.extraParams, {

});
});
function Query(x) {
    WCT8Store.removeAll();
    Ext.getCmp("gdFgroup").store.loadPage(1, {
        params: {
    }
});
}
Ext.onReady(function () {
    var gdFgroup = Ext.create('Ext.grid.Panel', {
        id: 'gdFgroup',
        store: WCT8Store,
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
            { header: BIGTITLE, dataIndex: 'big_title', width: 130, align: 'center' },        //大標
            { header: SMALLTITLE, dataIndex: 'small_title', width: 130, align: 'center' },    //小標
            {
                header: CONTENTIMAGE,
                dataIndex: 'home_image',
                width: 80,
                align: 'center',
                xtype: 'templatecolumn',
                tpl: '<img width=50 name="tplImg" height=50 src="{home_image}" />'
            },
            { header: LINKURL, dataIndex: 'link_url', width: 130, align: 'center' },
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
            { header: UPDATEON, dataIndex: 'update_on', width: 130, align: 'center' },
            { header: CREATEDON, dataIndex: 'created_on', width: 130, align: 'center' },
            { header: SORT, dataIndex: 'sort', width: 70, align: 'center' },
            { header: STARTTIME, dataIndex: 'start_time', width: 130, align: 'center' },
            { header: ENDTIME, dataIndex: 'end_time', width: 130, align: 'center' },
            {
                header: CONTENTSTATUS,
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
            { xtype: 'button', text: EDIT, id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: WCT8Store,
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
        items: [gdFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    WCT8Store.load({ params: { start: 0, limit: 10} });
});

/*************************新增**************************/
onAddClick = function () {
    editFunction(null, WCT8Store);
}

/**************************編輯*******************/
onEditClick = function () {
    var row = Ext.getCmp("gdFgroup").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], WCT8Store);
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
            storeType: "web_content_type8",
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
                    params: { id: id,
                        active: activeValue,
                        storeType: "type8"
                    },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            WCT8Store.load();
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
                storeType: "type8"
            },
            success: function (form, action) {
                var result = Ext.decode(form.responseText);
                if (result.success) {
                    WCT8Store.load();
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

