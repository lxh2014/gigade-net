var CallidForm;
var pageSize = 25;
//問卷Model
Ext.define('gigade.SiteStatistics', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "ss_id", type: "string" },
        { name: "ss_show_num", type: "string" },
        { name: "ss_click_num", type: "string" },
        { name: "ss_click_through", type: "string" },
        { name: "ss_cost", type: "string" },
        //{ name: "ss_budget", type: "string" },
        //{ name: "ss_effect_num", type: "string" },
        //{ name: "ss_rank", type: "string" },
        { name: "ss_newuser_number", type: "string" },
        { name: "ss_converted_newuser", type: "string" },
        { name: "ss_sum_order_amount", type: "string" },
        { name: "ss_date", type: "string" },
        { name: "ss_code", type: "string" },
        { name: "ss_create_time", type: "string" },
        { name: "ss_create_user", type: "string" },
        { name: "ss_modify_time", type: "string" },
        { name: "ss_modify_user", type: "string" }
    ]
});

var SiteStatisticsStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.SiteStatistics',
    proxy: {
        type: 'ajax',
        url: '/SiteManager/GetSiteStatisticsList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
var sm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections)
        {
            Ext.getCmp("gdSiteStatistics").down('#edit').setDisabled(selections.length == 0);
        }
    }
});
SiteStatisticsStore.on('beforeload', function ()
{
    Ext.apply(SiteStatisticsStore.proxy.extraParams, {
        ss_code: Ext.getCmp('s_code').getValue(),
        startdate: Ext.getCmp('startdate').getValue(),
        enddate: Ext.getCmp('enddate').getValue()
    });
});
Ext.apply(Ext.form.field.VTypes, {
    //日期筛选
    daterange: function (val, field)
    {
        var date = field.parseDate(val);
        if (!date)
        {
            return false;
        }
        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime())))
        {
            var start = field.up('form').down('#' + field.startDateField);
            start.setMaxValue(date);
            //start.validate();
            this.dateRangeMax = date;
        }
        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime())))
        {
            var end = field.up('form').down('#' + field.endDateField);
            end.setMinValue(date);
            //end.validate();
            this.dateRangeMin = date;
        }
        return true;
    },
    daterangeText: ''
});
Ext.define("gigade.paraModel", {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'parameterCode', type: 'string' },
        { name: 'parameterName', type: 'string' }
    ]
});
var scodeStore = Ext.create("Ext.data.Store", {
    model: 'gigade.paraModel',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Parameter/QueryPara?paraType=ss_code',
        noCache: false,
        getMethod: function () { return 'get'; },
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'items'
        }
    }
});
Ext.onReady(function ()
{
    Ext.QuickTips.init();
    var frm = Ext.create('Ext.form.Panel', {
        id: 'frm',
        layout: 'anchor',
        flex: 1.1,
        border: 0,
        bodyPadding: 10,
        url: '/SiteManager/GetSiteStatisticsList',
        width: document.documentElement.clientWidth,
        items: [
            {
                xtype: 'fieldcontainer',
                combineErrors: true,
                layout: 'hbox',
                items: [
                     {
                         xtype: 'combobox', //status
                         fieldLabel: '廠家代碼',
                         editable: false,
                         defaultListConfig: {              //取消loading的Mask
                             loadMask: false,
                             loadingHeight: 70,
                             minWidth: 30,
                             maxHeight: 300,
                             shadow: "sides"
                         },
                         id: 's_code',
                         margin: '0 0',
                         store: scodeStore,
                         displayField: 'parameterCode',
                         valueField: 'parameterCode',
                         typeAhead: true,
                         forceSelection: false,
                         queryMode: 'local',
                         allowBlank: true,
                         emptyText: '請選擇',
                         value: '',
                         labelWidth:60
                     },
                     {
                         xtype: 'displayfield',
                         fieldLabel: '查詢日期',
                         margin: '0 0 0 5',
                         labelWidth: 60
                     },
                    {
                        xtype: 'datefield',
                        id: 'startdate',
                        name: 'startdate',
                        margin: '0 5 0 0',
                        editable: false,
                        value: Tomorrow(1),
                        format: 'Y/m/d',
                        vtype: 'daterange',
                        endDateField: 'enddate'

                    },
                    {
                        xtype: 'displayfield',
                        value: '~'
                    },
                    {
                        xtype: 'datefield',
                        id: 'enddate',
                        name: 'enddate',
                        margin: '0 5',
                        editable: false,
                        value: new Date(),
                        format: 'Y/m/d',
                        vtype: 'daterange',
                        startDateField: 'startdate'
                    },
                    {
                        xtype: 'filefield',
                        id: 'ImportExcel',
                        width: 400,
                        labelWidth:70,
                        anchor: '80%',
                        name: 'ImportExcel',
                        fieldLabel: '匯入Excel',
                        buttonText: '選擇Excel...'
                    },
                    {
                        xtype: 'displayfield',
                        margin:'0 0 0 10',
                        value: '<a href="/Template/SiteStatistics/站臺訪問量統計匯入範本.xlsx">範例下載</a>'
                    },
                    {
                        xtype: 'button',
                        margin: '0 10 0 10',
                        text: "匯入",
                        handler: Import
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
                            margin: '0 10 0 10',
                            iconCls: 'ui-icon ui-icon-search-2',
                            text: "查詢",
                            handler: Query
                        },
                        {
                            xtype: 'button',
                            text: '重置',
                            id: 'btn_reset',  
                            iconCls: 'ui-icon ui-icon-reset',
                            listeners: {
                                click: function ()
                                {
                                    Ext.getCmp('s_code').setValue("");
                                    Ext.getCmp('startdate').reset();//開始時間--time_start--delivery_date
                                    Ext.getCmp('enddate').reset();//結束時間--time_end--delivery_date
                                    Ext.getCmp('startdate').setValue(Tomorrow(1));//開始時間--time_start--delivery_date
                                    Ext.getCmp('enddate').setValue(new Date());//結束時間--time_end--delivery_date
                                    Query(1);
                                }
                            }
                        }
                    ]
                }
        ]
    });
    var gdSiteStatistics = Ext.create('Ext.grid.Panel', {
        id: 'gdSiteStatistics',
        store: SiteStatisticsStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        flex: 8.9,
        columns: [
            { header: "流水號", dataIndex: 'ss_id', width: 50, align: 'center' },
            { header: "曝光數", dataIndex: 'ss_show_num', width: 150, align: 'center' },
            { header: "點閱數", dataIndex: 'ss_click_num', width: 100, align: 'center' },
            { header: "點閱率", dataIndex: 'ss_click_through', width: 80, align: 'center' },
            { header: "費用", dataIndex: 'ss_cost', width: 60, align: 'center' },
            //{ header: "預算", dataIndex: 'ss_budget', width: 150, align: 'center' },
            //{ header: "有效點閱數", dataIndex: 'ss_effect_num', width: 80, align: 'center' },
            //{ header: "平均排名", dataIndex: 'ss_rank', width: 100, align: 'center' },
            { header: "新會員數", dataIndex: 'ss_newuser_number', width: 150, align: 'center' },
            { header: "實際轉換", dataIndex: 'ss_converted_newuser', width: 80, align: 'center' },
            { header: "訂單金額", dataIndex: 'ss_sum_order_amount', width: 100, align: 'center' },
            {
                header: "時間", dataIndex: 'ss_date', width: 110, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return value.substr(0, 10);
                }
            },
            { header: "廠家代碼", dataIndex: 'ss_code', width: 150, align: 'center' },
            { header: "創建時間", dataIndex: 'ss_create_time', width: 150, align: 'center' },
            //{ header: "創建人", dataIndex: 'ss_create_user', width: 100, align: 'center' },
            {
                header: "刪除", dataIndex: 'ss_create_user', width: 100, align: 'center',id:'delete',hidden:true,
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store)
                {
                    return "<a href='javascript:void(0);' onclick='Delete(" + record.data.ss_id + ")'><img  src='../../../Content/img/icons/icon_delete.gif'/></a>";
                }
            }
        ],
        tbar: [
         { xtype: 'button', text: "新增", id: 'add', hidden: true, iconCls: 'icon-user-add', handler: onAddClick },
         { xtype: 'button', text: "編輯", id: 'edit', hidden: true, iconCls: 'icon-user-edit', disabled: true, handler: onEditClick },
         { xtype: 'button', text: "匯出Excel", id: 'outExcel', icon: '../../../Content/img/icons/excel.gif', handler: outExcel },
         '->',
         { text: ' ' }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: SiteStatisticsStore,
            pageSize: pageSize,
            displayInfo: true,
            displayMsg: NOW_DISPLAY_RECORD + ': {0} - {1}' + TOTAL + ': {2}',
            emptyMsg: NOTHING_DISPLAY
        }),
        listeners: {
            scrollershow: function (scroller)
            {
                if (scroller && scroller.scrollEl)
                {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'vbox',
        items: [frm, gdSiteStatistics],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function ()
            {
                gdSiteStatistics.width = document.documentElement.clientWidth;                
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    SiteStatisticsStore.load({ params: { start: 0, limit: 25 } });
});
/*************************************************************************************查詢*************************************************************************************************/
function Query(x)
{
    SiteStatisticsStore.removeAll();
    Ext.getCmp("gdSiteStatistics").store.loadPage(1, {
        params: {
            ss_code: Ext.getCmp('s_code').getValue(),
            startdate: Ext.getCmp('startdate').getValue(),
            enddate: Ext.getCmp('enddate').getValue()

        }
    });
}

function Import()
{
    var file = Ext.getCmp('ImportExcel').getValue();
    if (file != '') {
        var form = Ext.getCmp('frm').getForm();
        form.submit({
            waitMsg: '正在匯入...',
            params: {
                ImportExcel: Ext.htmlEncode(Ext.getCmp('ImportExcel').getValue())
            },
            success: function (form, action) {
                var result = Ext.decode(action.response.responseText);
                if (result.success) {
                    Ext.Msg.alert("提示信息", "匯入成功");
                    SiteStatisticsStore.load();
                }
                else {
                    Ext.Msg.alert("提示信息", "匯入失敗");
                }
            },
            failure: function () {
                Ext.Msg.alert("提示信息", "匯入失敗");
            }
        });
    }
    else {
        Ext.Msg.alert("提示信息", "請選擇Excel文件");
    }
        
    
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function ()
{
    //addWin.show();
    editSiteStatisticsFunction(null, SiteStatisticsStore);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function ()
{
    var row = Ext.getCmp("gdSiteStatistics").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0)
    {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1)
    {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1)
    {
        editSiteStatisticsFunction(row[0], SiteStatisticsStore);
    }
}
outExcel = function () {
    var params = 'ss_code=' + Ext.getCmp('s_code').getValue() + '&startdate=' + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('startdate').getValue()), 'Y-m-d')) + '&enddate=' + Ext.htmlEncode(Ext.Date.format(new Date(Ext.getCmp('enddate').getValue()), 'Y-m-d'))
    window.open('/SiteManager/ExportExcelStatistics?' + params);
}
//更改狀態(啟用或者禁用)
function Delete(id)
{

    Ext.MessageBox.confirm("提示信息",  "是否刪除此條數據", function (btn) {
        if (btn == "yes")
        {
            $.ajax({
                url: "/SiteManager/Delete",
                data: {
                    "id": id
                },
                type: "POST",
                dataType: "json",
                success: function (msg)
                {
                    SiteStatisticsStore.load();
                },
                error: function (msg)
                {
                    Ext.Msg.alert(INFORMATION, FAILURE);
                    SiteStatisticsStore.load();
                }
            });
        }
    });

}
function Tomorrow(month)
{
    var d;
    d = new Date();                             // 创建 Date 对象。
    //d.setDate(d.getDate() + days);
    d.setMonth(d.getMonth() - 1);
    return d;
}






