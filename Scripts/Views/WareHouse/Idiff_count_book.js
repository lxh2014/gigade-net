var pageSize = 25;
/**********************************************************************群組管理主頁面**************************************************************************************/
//群組管理Model
Ext.define('gigade.idiffcount', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "cb_jobid", type: "string" },
        { name: "loc_id", type: "string" },
        { name: "pro_qty", type: "int" },
        { name: "st_qty", type: "int" },
        { name: "user_username", type: "string" },
        { name: "create_time", type: "string" },
        { name: "item_id", type: "string" },
        { name: "made_date", type: "string" },
        { name: "cde_dt", type: "string" }
    ]
});

var IdiffcountStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.idiffcount',
    proxy: {
        type: 'ajax',
        url: '/WareHouse/GetIdiffCountBookList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
});
IdiffcountStore.on('beforeload', function () {
    Ext.apply(IdiffcountStore.proxy.extraParams,
        {
            s_job_id: Ext.getCmp('s_job_id').getValue(),
            s_item_id: Ext.getCmp('s_item_id').getValue(),
            s_loc_id: Ext.getCmp('s_loc_id').getValue()
        });
});
function Query(x) {
    IdiffcountStore.removeAll();
    var job_id = Ext.getCmp('s_job_id').getValue();
    var item_id = Ext.getCmp('s_item_id').getValue();
    var loc_id = Ext.getCmp('s_loc_id').getValue();
    if (job_id.trim() == "" && item_id.trim() == "" && loc_id.trim()=="")
    {
        Ext.Msg.alert("提示", "請輸入查詢條件");
        return;
    }
    Ext.getCmp("IdiffFgroup").store.loadPage(1, {
        params: {
            s_job_id:Ext.getCmp('s_job_id').getValue(),
            s_item_id: Ext.getCmp('s_item_id').getValue(),
            s_loc_id: Ext.getCmp('s_loc_id').getValue()
        }
    });
}
Ext.onReady(function () {
    var IdiffFgroup = Ext.create('Ext.grid.Panel', {
        id: 'IdiffFgroup',
        store: IdiffcountStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        viewConfig: {
            enableTextSelection: true,
            stripeRows: false,
            getRowClass: function (record, rowIndex, rowParams, store) {
                return "x-selectable";
            }
        },
        columns: [
            { header: "工作編號", dataIndex: 'cb_jobid', width: 120, align: 'center', align: 'center' },
            { header: "細項編號", dataIndex: 'item_id', width: 60, align: 'center', align: 'center' },
            { header: "料位", dataIndex: 'loc_id', width: 100, align: 'center' },
            { header: "庫存數量", dataIndex: 'pro_qty', width: 120, align: 'center' },
            { header: "復盤數量", dataIndex: 'st_qty', width: 80, align: 'center' },
            { header: "創造日期", dataIndex: 'made_date', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                     if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
            {
                header: "有效日期", dataIndex: 'cde_dt', width: 100, align: 'center', renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value != '' && value.length >= 10) {
                        return value.substring(0, 10);
                    }
                }
            },
            { header: "創建日期", dataIndex: 'create_time', width: 150, align: 'center' },
            { header: "創建人", dataIndex: 'user_username', width: 100, align: 'center' }
        ],
        tbar: [
            '->',
            { xtype: 'textfield', labelWidth: 60, allowBlank: true, id: 's_loc_id', fieldLabel: "料位" },
            { xtype: 'textfield', labelWidth: 60, allowBlank: true, id: 's_job_id', fieldLabel: "工作編號" },
            { xtype: 'textfield', labelWidth: 60, allowBlank: true, id: 's_item_id', fieldLabel: "細項編號", regex: /^\d*$/, regexText: "請輸入正確的細項編號類型" },

            {
                text: "查詢",
                iconCls: 'icon-search',
                id: 'btnQuery',
                handler: Query
            },
             {
                 xtype: 'button',
                 text: RESET,

                 margin: '0 0 0 10',
                 id: 'btn_reset',
                 iconCls: 'ui-icon ui-icon-reset',
                 listeners: {
                     click: function () {
                         Ext.getCmp('s_job_id').setValue("");
                         Ext.getCmp('s_item_id').setValue("");
                         Ext.getCmp('s_loc_id').setValue("");
                     }
                 }
             }
        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: IdiffcountStore,
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
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [IdiffFgroup],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                IdiffFgroup.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    //IdiffcountStore.load({ params: { start: 0, limit: 25 } });
});