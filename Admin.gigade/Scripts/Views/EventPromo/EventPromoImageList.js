pageSize = 25;
modifiable = false;
multi = 0;
isfirst = 0;
changeError = 0;
Ext.define('gigade.PromotionBanner', {
    extend: 'Ext.data.Model',
    fields: [
    { name: 'pb_id', type: 'int' },  
    { name: 'pb_image', type: 'string' },
    { name: 'pb_image_link', type: 'string' },
    { name: 'pb_startdate', type: 'string' },
    { name: 'pb_enddate', type: 'string' },
    { name: 'pb_status', type: 'int' },
    { name: 'pb_kdate', type: 'string' },
    { name: 'createusername', type: 'string' },
    { name: 'pb_mdate', type: 'string' },
    { name: 'updateusername', type: 'string' }
    ]
});

var EventPromoImageListStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.PromotionBanner',
    proxy: {
        type: 'ajax',
        url: '/EventPromo/GetPromotionBannerList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
var dateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { 'txt': '全部日期', 'value': '0' },
    { 'txt': '顯示開始時間', 'value': '1' },
    { 'txt': '顯示結束時間', 'value': '2' },
    { 'txt': '建立時間', 'value': '3' },
    { 'txt': '異動時間', 'value': '4' },
    ]
});
var statusStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { 'txt': '全部', 'value': '-1' },
    { 'txt': '未啟用', 'value': '0' },
    { 'txt': '已啟用', 'value': '1' }
    ]
});
var outOFDateStore = Ext.create('Ext.data.Store', {
    fields: ['txt', 'value'],
    data: [
    { 'txt': '全部', 'value': '0' },
    { 'txt': '未過期', 'value': '1' },
    { 'txt': '已過期', 'value': '2' }
    ]
});
EventPromoImageListStore.on('beforeload', function () {
    Ext.apply(EventPromoImageListStore.proxy.extraParams, {
        dateCon: Ext.getCmp('dateCon').getValue(),
        date_start: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('timestart').getValue()), 'Y-m-d H:i:s')),
        date_end: Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('timeend').getValue()), 'Y-m-d H:i:s')),
        activeStatus: Ext.getCmp('activeStatus').getValue(),
        showStatus: Ext.getCmp('showStatus').getValue(),
        brand_id: Ext.getCmp('id').getValue(),
        brand_name: Ext.getCmp('name').getValue()
    });
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("gdList").down('#edit').setDisabled(selections.length == 0);
            Ext.getCmp("gdList").down('#delete').setDisabled(selections.length == 0);
        }
    }
});

Ext.onReady(function () {
    var searchForm = Ext.create('Ext.form.Panel', {
        id: 'searchForm',
        border: 0,
        height: 120,
        layout: 'anchor',
        width: document.documentElement.clientWidth,
        items: [
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'textfield',
                fieldLabel: '品牌編號',
                name: 'id',
                id: 'id',
                width: 180,
                labelWidth: 60,
                margin: '5 10 0 5',
                regex: /^([0-9]{1,9})$/,
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'textfield',
                fieldLabel: '品牌名稱',
                name: 'name',
                id: 'name',
                width: 200,
                labelWidth: 60,
                margin: '5 10 0 5',
                listeners: {
                    specialkey: function (field, e) {
                        if (e.getKey() == e.ENTER) {
                            Query();
                        }
                    }
                }
            },
            {
                xtype: 'combobox',
                store: outOFDateStore,
                id: 'showStatus',
                fieldLabel: '顯示狀態',
                displayField: 'txt',
                valueField: 'value',
                width: 200,
                labelWidth: 60,
                margin: '5 10 0 5',
                forceSelection: false,
                editable: false,
                value: '0'
            },
            ]

        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'combobox',
                store: statusStore,
                id: 'activeStatus',
                fieldLabel: '啟用狀態',
                displayField: 'txt',
                valueField: 'value',
                width: 180,
                labelWidth: 60,
                margin: '5 10 0 5',
                forceSelection: false,
                editable: false,
                value: '-1'
            },
            {
                xtype: 'combobox',
                id: 'dateCon',
                name: 'dateCon',
                store: dateStore,
                displayField: 'txt',
                valueField: 'value',
                fieldLabel: '日期條件',
                value: '0',
                editable: false,
                labelWidth: 60,
                width: 180,
                margin: '5 10 0 5',
            },
            {
                xtype: 'datefield',
                allowBlank: true,
                id: 'timestart',
                format: 'Y-m-d',
                margin: '5 5 0 0',
                editable: false,
                labelWidth: 60,
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (end.getValue() == null) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        } else if (end.getValue() < start.getValue()) {
                            end.setValue(setNextMonth(start.getValue(), 1));
                        }
                    }
                }
            },
            {
                xtype: 'displayfield',
                margin: '5 0 0 0',
                value: "~"
            },
            {
                xtype: 'datefield',
                allowBlank: true,
                id: 'timeend',
                format: 'Y-m-d',
                editable: false,
                margin: '5 0 0 5',
                listeners: {
                    select: function (a, b, c) {
                        var start = Ext.getCmp("timestart");
                        var end = Ext.getCmp("timeend");
                        if (start.getValue() != "" && start.getValue() != null) {
                            if (end.getValue() < start.getValue()) {
                                start.setValue(setNextMonth(end.getValue(), -1));
                            }
                        }
                        else {
                            start.setValue(setNextMonth(end.getValue(), -1));
                        }
                    }
                }
            }

            ]
        },
        {
            xtype: 'fieldcontainer',
            layout: 'hbox',
            items: [
            {
                xtype: 'button',
                text: SEARCH,
                id: 'btnQuery',
                margin: '5 5 0 37',
                iconCls: 'ui-icon ui-icon-search-2',
                handler: Query
            },
            {
                xtype: 'button',
                text: RESET,
                id: 'btn_reset',
                iconCls: 'ui-icon ui-icon-reset',
                margin: '5 5 0 0',
                listeners: {
                    click: function () {
                        Ext.getCmp('dateCon').reset();
                        Ext.getCmp('activeStatus').reset();
                        Ext.getCmp('timestart').reset();
                        Ext.getCmp('timeend').reset();
                        Ext.getCmp('showStatus').reset();
                        Ext.getCmp('id').reset();
                        Ext.getCmp('name').reset();
                    }
                }
            },
            ]

        },
        ]
    });
    var gdList = Ext.create('Ext.grid.Panel', {
        id: 'gdList',
        store: EventPromoImageListStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8,
        columns: [
        { header: "流水號", dataIndex: 'pb_id', flex: 1, align: 'center' },
        {
            header: "品牌", flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return "<a href='javascript:void(0)' onclick='onRelationClick()'><img src='../../../Content/img/icons/application_view_list.png' /></a>"
            }
        },
        {
            header: "促銷圖片",
            dataIndex: 'pb_image',
            flex: 1,
            align: 'center',
            xtype: 'templatecolumn',
            tpl: '<a target="_blank" href="{pb_image}" ><img width=50 name="tplImg" height=50 src="{pb_image}" /></a>'

        },
        {
            header: "圖片連結地址", dataIndex: 'pb_image_link', flex: 2, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                return Ext.String.format('<a href="{0}" target="bank">{1}</a>', value, value);
            }
        },
        { header: "顯示開始時間", dataIndex: 'pb_startdate', flex: 2, align: 'center' },
        { header: "顯示結束時間", dataIndex: 'pb_enddate', flex: 2, align: 'center' },
        { header: "建立時間", dataIndex: 'pb_kdate', flex: 2, align: 'center' },
        { header: "建立人員", dataIndex: 'createusername', flex: 1, align: 'center' },
        { header: "異動時間", dataIndex: 'pb_mdate', flex: 2, align: 'center' },
        { header: "異動人員", dataIndex: 'updateusername', flex: 1, align: 'center' },
        {
            header: "啟用狀態", dataIndex: 'pb_status', flex: 1, align: 'center',
            renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                if (value == 1) {
                    return "<a href='javascript:void(0);' onclick='UpdateStatus(" + record.data.pb_id + ")'><img hidValue='0' id='img" + record.data.pb_id + "' src='../../../Content/img/icons/accept.gif'/></a>";
                }
                else {
                    return "<a href='javascript:void(0);' onclick='UpdateStatus(" + record.data.pb_id + ")'><img hidValue='1' id='img" + record.data.pb_id + "' src='../../../Content/img/icons/drop-no.gif'/></a>";
                }
            }
        }
        ],
        tbar: [
        {
            xtype: 'button',
            text: ADD,
            id: 'add',
            iconCls: 'icon-user-add',
            handler: onAddClick
        },
        {
            xtype: 'button',
            text: EDIT,
            id: 'edit',
            iconCls: 'icon-user-edit',
            disabled: true,
            handler: onEditClick
        },
        {
            xtype: 'button',
            text: '刪除',
            id: 'delete',
            iconCls: 'icon-user-remove',
            disabled: true,
            hidden: true,
            handler: onDeleteClick
        }, '->',
        {
            xtype: 'button',
            text: '允許多圖',
            id: 'allowMulti',
            iconCls: 'icon-user-edit',
            hidden: true,
            handler: onAllowMultiClick
        },
        {
            xtype: 'button',
            text: '禁止多圖',
            id: 'forbidMulti',
            iconCls: 'icon-user-edit',
            hidden: true,
            handler: onForbidMultiClick
        }
        ],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: EventPromoImageListStore,
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
        layout: 'vbox',
        items: [searchForm, gdList],
        renderTo: Ext.getBody(),
        listeners: {
            resize: function () {
                gdList.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    AllowMultiOrNot();
    isfirst = 1;
})

//查询
Query = function () {
    EventPromoImageListStore.removeAll();
    if (!Ext.getCmp('id').regex.test(Ext.getCmp('id').getValue())) {
        Ext.Msg.alert(INFORMATION, "請輸入有效字符串");
        return;
    }
    if (Ext.getCmp('name').getValue() != "" && Ext.getCmp('name').getValue().trim() == "") {
        Ext.Msg.alert(INFORMATION, "請輸入有效字符串");
        return;
    }
    if (Ext.getCmp('dateCon').getValue() != "0") {
        if (Ext.getCmp('timestart').getValue() == ("" || null)) {
            Ext.Msg.alert(INFORMATION, "請選擇查詢日期");
            return;
        }
        if (Ext.getCmp('timeend').getValue() == ("" || null)) {
            Ext.Msg.alert(INFORMATION, "請選擇查詢日期");
            return;
        }
    }
    if (Ext.getCmp('dateCon').getValue() == "0" && (Ext.getCmp('timestart').getValue() != null || Ext.getCmp('timeend').getValue() != null)) {
        Ext.Msg.alert(INFORMATION, "請選擇日期條件");
        return;
    }
    else {
        Ext.getCmp("gdList").store.loadPage(1);
    }
}
function UpdateStatus(id) {
    var activeValue = $("#img" + id).attr("hidValue");
    if (Ext.getCmp("allowMulti").hidden) {
        multi = 1;
    }
    else {
        multi = 0;
    }
    $.ajax({
        url: "/EventPromo/UpdateStatus",
        data: {
            "id": id,
            "status": activeValue,
            "multi": multi
        },
        type: "POST",
        dataType: "json",
        success: function (msg) {
            if (msg.success == "true") {
                if (activeValue == 1) {
                    $("#img" + id).attr("hidValue", 0);
                    $("#img" + id).attr("src", "../../../Content/img/icons/accept.gif");
                    EventPromoImageListStore.load();
                }
                else {
                    $("#img" + id).attr("hidValue", 1);
                    $("#img" + id).attr("src", "../../../Content/img/icons/drop-no.gif");
                    EventPromoImageListStore.load();
                }
            }
            else {
                if (msg.error == "-1") {
                    Ext.Msg.alert(INFORMATION, '促銷圖片已過期, 請確認');
                }
                else if (msg.error == "-2") {
                    Ext.Msg.alert(INFORMATION, '品牌編號: ' + msg.id + ',在指定的時段內正在使用其他促銷圖片, 請確認');
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            }
        },
        error: function (msg) {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
onRelationClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
    else if (row.length == 1) {
        RelationFunction(row[0]);
    }
}
onAddClick = function () {
    editFunction(null, EventPromoImageListStore, multi);
}
onEditClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    }
    else if (row.length == 1) {
        IsModifiable(row[0].data.pb_id);
        if (modifiable) {
            editFunction(row[0], EventPromoImageListStore, multi);
        }
    }
}
onDeleteClick = function () {
    var row = Ext.getCmp("gdList").getSelectionModel().getSelection();
    if (row.length < 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                var ids = '';
                for (var i = 0; i < row.length; i++) {
                    ids += row[i].data.pb_id + '|';
                }
                Ext.Ajax.request({
                    url: '/EventPromo/DeleteImage',
                    method: 'post',
                    params: { ids: ids },
                    success: function (form, action) {
                        var result = Ext.decode(form.responseText);
                        if (result.success) {
                            Ext.Msg.alert(INFORMATION, SUCCESS);
                            EventPromoImageListStore.load();
                        }
                        else {
                            Ext.Msg.alert(INFORMATION, "刪除失敗");
                            EventPromoImageListStore.load();
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
IsModifiable = function (pb_id) {
    modifiable = false;
    Ext.Ajax.request({
        url: '/EventPromo/IsModifiable',
        method: 'post',
        async: false,
        params: {
            pb_id: pb_id
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                modifiable = true;
            }
            else {
                if (result.msg == '-1') {
                    Ext.Msg.alert(INFORMATION, "促銷圖片已過期，請確認");
                }
                else {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                }
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
setNextMonth = function (source, n) {
    var s = new Date(source);
    s.setMonth(s.getMonth() + n);
    if (n < 0) {
        s.setHours(0, 0, 0);
    }
    else if (n > 0) {
        s.setHours(23, 59, 59);
    }
    return s;
}
onAllowMultiClick = function () {
    multi = 1;
    Ext.Msg.alert(INFORMATION, '您將允許一個品牌添加多張促銷圖片');
    Ext.getCmp('allowMulti').hide();
    Ext.getCmp('forbidMulti').show();
}
onForbidMultiClick = function () {
    ChangeImageMode();
    if (multi == 1) {
        if (changeError == 0) {
            multi = 0;
            Ext.Msg.alert(INFORMATION, '您將禁止一個品牌添加多張促銷圖片');
            Ext.getCmp('forbidMulti').hide();
            Ext.getCmp('allowMulti').show();
        }
    }
}
AllowMultiOrNot = function () {
    multi = 0;
    ismulti = 0;
    notmulti = 0;
    Ext.Ajax.request({
        url: '/EventPromo/AllowMultiOrNot',
        method: 'post',
        async: false,
        params: {
            change: 'false',
        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.msg == '0') {
                //0是禁止多圖狀態，此時應該顯示可以多圖的按鈕
                Ext.getCmp('forbidMulti').hide();
                Ext.getCmp('allowMulti').show();
            }
            else {
                Ext.getCmp('forbidMulti').show();
                Ext.getCmp('allowMulti').hide();
                multi = 1;
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
        }
    });
}
ChangeImageMode = function () {
    changeError = 0;
    Ext.Ajax.request({
        url: '/EventPromo/AllowMultiOrNot',
        method: 'post',
        async: false,
        params: {
            change: 'true',

        },
        success: function (form, action) {
            var result = Ext.decode(form.responseText);
            if (result.success) {
                multi = 0;
                Ext.getCmp('forbidMulti').hide();
                Ext.getCmp('allowMulti').show();
            }
            else if (result.id != '0') {
                Ext.Msg.alert(INFORMATION, '品牌編號: ' + result.id + ',在指定的時段內正在使用其他促銷圖片, 請確認');
                multi = 1;
                changeError = 1;
                Ext.getCmp('forbidMulti').show();
                Ext.getCmp('allowMulti').hide();
            }
        },
        failure: function () {
            Ext.Msg.alert(INFORMATION, FAILURE);
            multi = 1;
        }
    });
}

