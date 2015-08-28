
var CallidForm;
var pageSize = 25;
/**********************************************************************站臺管理主頁面**************************************************************************************/
//站臺管理Model
Ext.define('gigade.Question', {
    extend: 'Ext.data.Model',
    fields: [
        { name: "question_username", type: "string" },
        { name: "question_type", type: "int" },
        { name: "question_createdates", type: "string" },
        { name: "question_phone", type: "string" },
        { name: "question_email", type: "string" },
        { name: "response_createdates", type: "string" },
        { name: "question_content", type: "string" },
        { name: "response_content", type: "string" },
         { name: "question_type_str", type: "string" },
        
    ]
});

var QuestionStore = Ext.create('Ext.data.Store', {
    autoDestroy: true,
    pageSize: pageSize,
    model: 'gigade.Question',
    proxy: {
        type: 'ajax',
        url: '/OrderManage/GetQuestion',
        reader: {
            type: 'json',
            root: 'data',
            totalProperty: 'totalCount'
        }
    }
    //    autoLoad: true
});
QuestionStore.on('beforeload', function () {
    Ext.apply(QuestionStore.proxy.extraParams,
        {
            Order_Id: window.parent.GetOrderId()

        });
});
Ext.onReady(function () {
    var QuestionGrid = Ext.create('Ext.grid.Panel', {
        id: 'QuestionGrid',
        store: QuestionStore,
        width: document.documentElement.clientWidth,
        columnLines: true,
        frame: true,
        columns: [
            { header: "姓名", dataIndex: 'question_username', width: 100, align: 'center' },
            { header: "問題分類", dataIndex: 'question_type_str', width: 100, align: 'center' },
            {
                header: "留言日期", dataIndex: 'question_createdates', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == '0001-01-01') {
                        return "-"
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "聯絡電話", dataIndex: 'question_phone', width: 100, align: 'center' },
            { header: "電子郵件", dataIndex: 'question_email', width: 100, align: 'center' },
            {
                header: "回覆日期", dataIndex: 'response_createdates', width: 150, align: 'center',
                renderer: function (value, cellmeta, record, rowIndex, columnIndex, store) {
                    if (value.substr(0, 10) == '0001-01-01') {
                        return "-"
                    }
                    else {
                        return value;
                    }
                }
            },
            { header: "留言內容", dataIndex: 'question_content', width: 400, align: 'center' },
            { header: "回覆內容", dataIndex: 'response_content', width: 500, align: 'center' }

        ],

        bbar: Ext.create('Ext.PagingToolbar', {
            store: QuestionStore,
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
        items: [QuestionGrid],
        renderTo: Ext.getBody(),
        autoScroll: true,
        listeners: {
            resize: function () {
                QuestionGrid.width = document.documentElement.clientWidth;
                this.doLayout();
            }
        }
    });
    ToolAuthority();
    QuestionStore.load({ params: { start: 0, limit: 25 } });
});





