var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//料位管理Model
Ext.define('gigade.Ilocs', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "boiler_type", type: "string" },
        { name: "boiler_describe", type: "string" },
        { name: "inner_boiler_number", type: "string" },
        { name: "out_boiler_number", type: "string" },
        { name: "user_username", type: "string" },
        { name: "add_time", type: "string" },
        { name: "boiler_remark",type:"string" }
    ]
});

var BoilerRelation = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Ilocs',
    proxy: {
        type: 'ajax',
        url: '/BoilerRelation/GetBoilerRelationList',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});

BoilerRelation.on('beforeload', function () {
    Ext.apply(BoilerRelation.proxy.extraParams, {
        out_boiler_type: Ext.getCmp("out_boiler_type").getValue(),
        innner_boiler_type: Ext.getCmp('innner_boiler_type').getValue(),
        boiler_type_describe: Ext.getCmp('boiler_type_describe').getValue()
    });
});

//var sm = Ext.create('Ext.selection.CheckboxModel', {
//    listeners: {
//        selectionchange: function (sm, selections) {
//            Ext.getCmp("gdBoilerRelation").down('#edit').setDisabled(selections.length == 0);
//            Ext.getCmp("gdBoilerRelation").down('#delete').setDisabled(selections.length == 0);
//        }
//    }
//});

Ext.onReady(function () {
    var gdBoilerRelation = Ext.create('Ext.grid.Panel', {
        id: 'gdBoilerRelation',
        store: BoilerRelation,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "外鍋型號", dataIndex: 'out_boiler_number', width: 222, align: 'center' },
            { header: "內鍋型號", dataIndex: 'inner_boiler_number', width: 222, align: 'center' },
            { header: "安康內鍋型號", dataIndex: 'boiler_type', width: 222, align: 'center' },
            { header: "安康內鍋型號信息", dataIndex: 'boiler_describe', width: 222, align: 'center' },
            { header: "備註", dataIndex: 'boiler_remark', width: 222, align: 'center' },
            { header: "添加人", dataIndex: 'user_username', width: 100, align: 'center' },
            { header: "添加時間", dataIndex: 'add_time', width: 222, align: 'center' }
        ],
        tbar: [
           
        ],
        dockedItems: [
       {   //類似于tbar
           xtype: 'toolbar',
           dock: 'top',
           items: [
               '->',
               { xtype: 'textfield', allowBlank: true, fieldLabel: "外鍋型號", id: 'out_boiler_type', name: 'out_boiler_type', labelWidth: 60 },
               { xtype: 'textfield', allowBlank: true, fieldLabel: "內鍋型號", id: 'innner_boiler_type', name: 'innner_boiler_type', labelWidth: 60 },
               { xtype: 'textfield', allowBlank: true, fieldLabel: "對應安康內鍋型號", id: 'boiler_type_describe', name: 'boiler_type_describe', labelWidth: 110 },
               {
                   xtype: 'button',
                   text: "查詢",
                   iconCls: 'icon-search',
                   id: 'btnQuery',
                   handler: Query
               },
               {
                   xtype: 'button',
                   text: "重置",
                   id: 'btn_reset',
                   listeners: {
                       click: function () {
                           Ext.getCmp("out_boiler_type").setValue("");
                           Ext.getCmp('innner_boiler_type').setValue("");
                           Ext.getCmp('boiler_type_describe').setValue("")
                       }
                   }
               }
           ]
       }],
        bbar: Ext.create('Ext.PagingToolbar', {
            store: BoilerRelation,
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
        //,
        //selModel: sm
    });

    Ext.create('Ext.container.Viewport', {
        layout: 'fit',
        items: [gdBoilerRelation],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                gdBoilerRelation.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    BoilerRelation.load({ params: { start: 0, limit: 25 } });
});


function Query(x) {
    BoilerRelation.removeAll();
    Ext.getCmp("gdBoilerRelation").store.loadPage(1, {
        params: {
            out_boiler_type:Ext.getCmp("out_boiler_type").getValue(),
            innner_boiler_type: Ext.getCmp('innner_boiler_type').getValue(),
            boiler_type_describe: Ext.getCmp('boiler_type_describe').getValue()
        }
    });
}
/*************************************************************************************新增*************************************************************************************************/
onAddClick = function () {
    //addWin.show();
    editFunction(null, BoilerRelation);
}

/*************************************************************************************編輯*************************************************************************************************/
onEditClick = function () {
    var row = Ext.getCmp("gdBoilerRelation").getSelectionModel().getSelection();
    //alert(row[0]);
    if (row.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    } else if (row.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (row.length == 1) {
        editFunction(row[0], BoilerRelation);
    }
}



