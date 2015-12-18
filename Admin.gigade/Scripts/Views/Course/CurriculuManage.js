
var currentRecord = { data: {} };
var pageSize = 19;
var win;

Ext.Loader.setConfig({ enabled: true });

//Ext.Loader.setPath('Ext.ux.DataView', '../ux/DataView/');

Ext.require([
    'Ext.data.*',
    'Ext.util.*',
    'Ext.view.View',
    'Ext.ux.DataView.DragSelector',
    'Ext.ux.DataView.LabelEditor'
]);



//courseList 課程列表
Ext.define('GIGADE.COURSE', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Course_Id', type: 'int' },
        { name: 'Course_Name', type: 'string' },
        { name: 'Product_Id', type: 'int' },
        { name: 'Tel', type: 'string' },
        { name: 'Send_Msg', type: 'int' },
        { name: 'Msg', type: 'string' },
        { name: 'Send_Mail', type: 'int' },
        { name: 'Mail_Content', type: 'string' },
        { name: 'Start_Date', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'End_Date', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'Create_Time', type: 'date' },
        { name: 'Source', type: 'int' },
        { name: 'Ticket_Type', type: 'int' }]
});

var courseListStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.COURSE',
    autoLoad: true,
    pageSize: pageSize,
    proxy: {
        type: 'ajax',
        url: '/Course/GetCurricul?IsPage=true',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'item',
            totalProperty: 'totalCount'
        }
    }
});

courseListStore.on("beforeload", function () {
    Ext.apply(courseListStore.proxy.extraParams, {
        Course_Id: Ext.getCmp("grid_course_id") ? Ext.getCmp("grid_course_id").getValue() : '',
        Course_Name: Ext.getCmp('grid_course_name') ? Ext.getCmp('grid_course_name').getValue() : ''
    })
})

// CourseDetailList 詳情
Ext.define('GIGADE.COURSEDETAIL', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Course_Detail_Id', type: 'int' },
        { name: 'Course_Id', type: 'int' },
        { name: 'Course_Detail_Name', type: 'string' },
        { name: 'Address', type: 'string' },
        { name: 'Start_Date', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'End_Date', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'P_Number', type: 'int' },
        { name: 'P_NumberReality', type: 'int' }]
});

var courseDetailStore = Ext.create('Ext.data.Store', {
    model: 'GIGADE.COURSEDETAIL',
    autoLoad: true,
    proxy: {
        type: 'ajax',
        url: '/Course/GetCurriculDetail',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});

// CourseTicketList 票劵
Ext.define('GIGADE.COURSETICKET', {
    extend: 'Ext.data.Model',
    autoLoad: true,
    fields: [
        { name: 'Ticket_Id', type: 'int' },
        { name: 'Course_Detail_Id', type: 'int' },
        { name: 'Ticket_Code', type: 'string' },
        { name: 'User_Id', type: 'int' },
        { name: 'User_Name', type: 'string' },
        { name: 'Create_Date', type: 'date', dateFormat: "Y-m-d H:i:s" },
        { name: 'Create_User', type: 'string' },
        { name: 'Order_Id', type: 'int' },
        { name: 'Flag', type: 'int' },
        { name: 'FlagName', type: 'string' }]
});

var courseTicketStore = Ext.create('Ext.data.Store', {//edit by wwei0216w 2015/6/2 暫不需要關於票據的代碼,將其去掉(後臺代碼仍有關於屬性)
    model: 'GIGADE.COURSETICKET',
    proxy: {
        type: 'ajax',
        url: '/Course/GetCurriculTicket',
        actionMethods: 'post',
        reader: {
            type: 'json'
        }
    }
});


Ext.onReady(function () {
    Ext.QuickTips.init();
    Ext.create('Ext.Viewport', {
        id: "index",
        autoScroll: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'border',
        items: [{
            region: 'west',//左西
            xtype: 'panel',
            autoScroll: true,
            frame: false,
            width: 400,
            margins: '5 4 5 5',
            //collapsible: true,
            id: 'west-region-container',
            layout: 'anchor',
            items: courselist
        }, {
            region: 'center',//中間
            id: 'center-region-container',
            xtype: 'panel',
            frame: false,
            layout: 'fit',
            width: 500,
            margins: '5 4 5 5',
            items: center
        }],
        listeners: {
            resize: function () {
                Ext.getCmp("index").width = document.documentElement.clientWidth;
                Ext.getCmp("index").height = document.documentElement.clientHeight;
                this.doLayout();
            },
            scrollershow: function (scroller) {
                if (scroller && scroller.scrollEl) {
                    scroller.clearManagedListeners();
                    scroller.mon(scroller.scrollEl, 'scroll', scroller.onElScroll, scroller);
                }
            }
        },
        renderTo: Ext.getBody()
    });

});

//左邊課程列表
var courselist = Ext.create('Ext.grid.Panel', {
    id: ' courselist',
    autoScroll: true,
    layout: 'anchor',
    height: document.documentElement.clientHeight - 12,
    border: false,
    frame: false,
    store: courseListStore,
    dockedItems: [{
        id: 'dockedItem',
        xtype: 'toolbar',
        layout: 'column',
        dock: 'top',
        items: [{
            xtype: 'textfield',
            fieldLabel: CURRICULUM_CODE,//課程編號
            id: 'grid_course_id',
            name: 'course_id',
            margin: '3 0 0 0',
            width: 210,
            labelWidth: 60
        }, {
            xtype: 'textfield',
            fieldLabel: CURRICULUM_NAME,//課程名稱
            id: 'grid_course_name',
            name: 'course_name',
            width: 210,
            labelWidth: 60,
            margin: '3 0 5 0'
        }, {
            xtype: 'button',
            text: QUERY,//查詢
            id: 'grid_btn_search',
            iconCls: 'ui-icon ui-icon-search',
            margin: ' 0 0 5 10',
            width: 65,
            handler: Search
        }, {
            xtype: 'button',
            text: CREAT_NEW_CURRICULUM,//新增课程
            id: 'grid_btn_add',
            iconCls: 'ui-icon ui-icon-add',
            margin: ' 0 0 5 10',
            width: 75,
            handler: function () {
                var record = { data: { 'Course_Id': 0 } };
                currentRecord = record;
                Ext.getCmp('west-region-container').setDisabled(true);
                Ext.getCmp('center').getForm().reset();
                courseDetailStore.removeAll();
                //courseTicketStore.removeAll();//edit by wwei0216w 2015/6/2 暫不需要關於票據的代碼,將其去掉(後臺代碼仍有關於屬性)
                imgstore.removeAll();
            }
        }]
    }],
    columns: [
        { xtype: 'rownumberer', width: 30, align: 'center' },
        //{ header: '編號', dataIndex: 'Course_Id', align: 'left', width: 40, menuDisabled: true, sortable: false },
        { header: CURRICULUM_NAME, dataIndex: 'Course_Name', align: 'left', width: 138, menuDisabled: true, sortable: false, flex: 1 },//課程名稱
        { header: BEGIN_DATE, dataIndex: 'Start_Date', align: 'center', width: 110, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d <br> H:i:s') },//開始時間
        { header: END_DATE, dataIndex: 'End_Date', align: 'center', width: 110, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d <br> H:i:s') }],//結束時間
    bbar: Ext.create('Ext.PagingToolbar', {
        store: courseListStore,
        dock: 'bottom',
        pageSize: pageSize,
        displayInfo: true
    }),
    listeners: {
        itemclick: function (grid, record) {
            //courseTicketStore.removeAll();//edit by wwei0216w 2015/6/2 暫不需要關於票據的代碼,將其去掉(後臺代碼仍有關於屬性)
            LoadDetail(currentRecord = record);
        },
        resize: function () {
            this.doLayout();
        }
    }
})

//複選框列
var cbm = Ext.create('Ext.selection.CheckboxModel', {
    listeners: {
        selectionchange: function (sm, selections) {
            Ext.getCmp("edit").setDisabled(selections.length == 0);
            Ext.getCmp("delete").setDisabled(selections.length == 0);
        }
    }
});

//日期筛选
//Ext.apply(Ext.form.field.VTypes, {
//    daterange: function (val, field) {
//        var date = field.parseDate(val);
//        if (!date) {
//            return false;
//        }
//        if (field.startDateField && (!this.dateRangeMax || (date.getTime() != this.dateRangeMax.getTime()))) {
//            var start = Ext.getCmp(field.startDateField);
//            start.setMaxValue(date);
//            start.validate();
//            this.dateRangeMax = date;
//        }
//        else if (field.endDateField && (!this.dateRangeMin || (date.getTime() != this.dateRangeMin.getTime()))) {
//            var end = Ext.getCmp(field.endDateField);
//            end.setMinValue(date);
//            end.validate();
//            this.dateRangeMin = date;
//        }
//        return true;
//    },
//    daterangeText: '開始時間必須小於結束時間'
//});


Ext.define('ImageModel', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'id', type: 'int' },
        { name: 'course_id', type: 'int' },
        { name: 'picture_name', type: 'string' },
        { name: 'picture_type', type: 'string' },
        { name: 'picture_status', type: 'int' },
        { name: 'picture_sort', type: 'int' }
    ]
});

var imgstore = Ext.create('Ext.data.Store', {
    model: 'ImageModel',
    id: 'imgStore',
    proxy: {
        type: 'ajax',
        url: '/Course/GetCourseImg',
        actionMethods: 'post',
        reader: {
            type: 'json',
            root: 'images'
        }
    }
});

var imgView = Ext.create('Ext.Panel', {
    //id: 'images-view',
    frame: false,
    width: 1000,
    margin: '0 0 0 5',
    height: 86,
    emptyText: NOW_NO_IMAGE_DATA,//暫無圖片數據
    autoScroll: true,
    items: [{
        xtype: "dataview",
        store: 'imgStore',
        itemSelector: 'div.thumb-wrap',
        trackOver: true,
        tpl: [
            '<tpl for=".">',
                '<div class="thumb-wrap" style="float:left;">',
                    '<img src="{picture_name}" data-qtip="<img src=\'{picture_name}\' style=\'max-width:100%;height:auto; \' />" style="width:80px;height:80px" />',
                    '<lable style="color:red; font-size:1.5em;font-weight: bold; cursor:pointer" onclick="imgCancel({id})" >×</lable>',
                '</div>',
            '</tpl>'
        ]
    }]
});

function imgCancel(id) {
    imgstore.removeAt(imgstore.find("id", id));
}


var center = Ext.create('Ext.form.Panel', {
    id: 'center',
    autoScroll: true,
    border: false,
    frame: false,
    layout: { type: 'vbox', align: 'stretch' },
    defaults: { margin: '2 2 2 2' },
    items: [{
        flex: 2.0,
        title: '基本資料',
        autoScroll: true,
        frame: false,
        //defaults: { margin: '5 5 5 10', labelWidth: 30 },
        items: [{
            xtype: 'container',
            autoScroll: true,
            defaults: { margin: '0 5 5 10', labelWidth: 60, autoScroll: true, width: 1150 },
            items: [{
                xtype: 'container',
                layout: 'hbox',
                autoScroll: true,
                defaults: { margin: '3 5 5 10', labelWidth: 60, width: 220 },
                id: 'part1',
                items: [{
                    xtype: 'textfield',
                    id: 'course_name',//課程名稱
                    fieldLabel: CURRICULUM_NAME,
                    margin: '3 5 5 0',
                    allowBlank: false
                }, {
                    xtype: 'textfield',
                    id: 'tel',//聯繫電話
                    allowBlank: false,
                    fieldLabel: CONN_TEL
                }, {
                    xtype: 'datetimefield',
                    format: 'Y-m-d H:i:s',
                    time: { hour: 00, min: 00, sec: 00 },
                    id: 'start_date',
                    allowBlank: false,
                    editable: false,
                    celleditable: false,
                    listeners: {//change by shiwei0620j 20151217,將時間控件改為可以選擇時分秒，開始時間時分秒默認為00:00:00,結束時間時分秒默認為23:59:59，當選擇的開始時間大於結束時間，結束時間在開始時間月份加1，當選擇的結束時間大於開始時間，開始時間在結束時間月份加1;
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_date");
                            var end = Ext.getCmp("end_date");
                            var start_date = start.getValue();
                            if (end.getValue() == ""||end.getValue() == null) {
                                Ext.getCmp('end_date').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                            }
                            else if (end.getValue() < start.getValue()) {
                                Ext.getCmp('end_date').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                            }
                        }
                    },
                    //vtype: 'daterange',
                    //endDateField: 'end_date',
                    //minValue: new Date(),
                    fieldLabel: BEGIN_DATE//開始時間
                }, {
                    xtype: 'datetimefield',
                    format: 'Y-m-d H:i:s',
                    time: { hour: 23, min: 59, sec: 59 },
                    id: 'end_date',
                    allowBlank: false,
                    editable: false,
                    celleditable: false,
                    listeners: {
                        select: function (a, b, c) {
                            var start = Ext.getCmp("start_date");
                            var end = Ext.getCmp("end_date");
                            var end_date = end.getValue();
                            if (start.getValue() == "" || start.getValue() == null) {
                                Ext.getCmp('start_date').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                            }
                            if (end.getValue() < start.getValue()) {
                                Ext.getCmp('start_date').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                            }
                        }
                    },
                    //vtype: 'daterange',
                    //startDateField: 'start_date',
                   // minValue: new Date(),
                    fieldLabel: END_DATE//結束時間
                }]
            }, {
                xtype: 'container',
                layout: 'hbox',
                id: 'part2',
                items: [{
                    xtype: 'container',
                    id: 'part2-1',
                    defaults: { width: 320 },
                    items: [{
                        xtype: 'checkbox',
                        boxLabel: AFFIRM_SHOW_MESSAGE,//是否發送簡訊
                        width: 100,
                        inputValue: '0',
                        id: 'send_msg',
                        listeners: {
                            change: function (cb) {
                                if (!cb.getValue()) {
                                    Ext.getCmp('msg').setDisabled(true);
                                } else {
                                    Ext.getCmp('msg').setDisabled(false);
                                }
                            }
                        }
                    }, {
                        xtype: 'textarea',
                        fieldLabel: TEXT_MESSAGE,//簡訊內容
                        disabled: true,
                        id: 'msg'
                    }]
                }, {
                    xtype: 'container',
                    id: 'part2-2',
                    margin: '0 0 0 20',
                    defaults: { width: 320 },
                    items: [{
                        xtype: 'checkbox',
                        boxLabel: AFFIRM_SHOW_MAIL,//是否發送Mail
                        width: 100,
                        inputValue: '0',
                        id: 'send_mail',
                        listeners: {
                            change: function (cb) {
                                if (!cb.getValue()) {
                                    Ext.getCmp('mail_content').setDisabled(true);
                                } else {
                                    Ext.getCmp('mail_content').setDisabled(false);
                                }
                            }
                        }
                    }, {
                        xtype: 'textarea',
                        fieldLabel: TEXT_MAIL,//Mail內容
                        disabled: true,
                        height: 50,
                        id: 'mail_content'
                    }]
                }, {
                    xtype: 'container',
                    margin: '3 0 0 20',
                    //layout: 'hbox',
                    id: 'part2-3',
                    items: [{
                        xtype: 'radiogroup',
                        fieldLabel: CURRICULUM_FROM,//課程來源
                        width: 260,
                        id: 'source',
                        columns: 2,
                        vertical: true,
                        defaults: { name: 'source' },
                        items: [
                            { boxLabel: PERSONAL_TRANSACT, id: 'zb', inputValue: 0, checked: true },//自辦
                            { boxLabel: COOPERATION_BUSINESS, id: 'hzs', inputValue: 1 }]//合作商
                    }, {
                        xtype: 'radiogroup',
                        fieldLabel: TICKET_TYPE,//票劵類型
                        width: 260,
                        id: 'ticket_type',
                        columns: 2,
                        vertical: true,
                        defaults: { name: 'ticket_type' },
                        items: [
                            { boxLabel: VIRTUAL, id: 'xn', inputValue: 0, checked: true },//虛擬
                            { boxLabel: SUBSTANCE, id: 'st', inputValue: 1 }] //實體
                    }]
                }]
            }, {
                xtype: 'container',
                layout: 'hbox',
                id: 'part3',
                items: [{
                    xtype: 'button',
                    id: 'addpic',
                    text: QUANTITY_UPLOADING_IMAGE_BR,//<h2>批量<br>上傳<br>圖片</h2>
                    width: 50,
                    height: 87,
                    //scale: 'medium',
                    iconCls: 'icon-add',
                    iconAlign: 'top',
                    handler: function () {
                        if (!win) {
                            win = Ext.create('Ext.window.Window', {
                                title: QUANTITY_UPLOADING_IMAGE,//批量上傳圖片
                                height: 450,
                                frame: false,
                                border: false,
                                width: 410,
                                listeners: {
                                    close: function (e) {
                                        win = undefined;
                                    }
                                },
                                tbar: [{
                                    //html: '<input type="file" id="uploadify" name = "uploadify" />',
                                    text: '<h1>' + SELECT_IMAGE + '</h1>',//選擇圖片
                                    id: 'uploadify',
                                    width: 118,
                                    height: 35
                                }, {
                                    xtype: 'button',
                                    text: BEGIN_UPLOADING,//開始上傳
                                    handler: function () {
                                        $("#uploadify").uploadifyUpload();
                                    }
                                }, {
                                    xtype: 'button',
                                    text: CANCEL_UPLOADING,//取消上傳
                                    handler: function () {
                                        $('#uploadify').uploadifyClearQueue();
                                    }
                                }],
                                items: [{
                                    html: "<div id='fileQueue' style='width: 450px;height: 400px;overflow: auto;border: 1px solid #E5E5E5;margin-bottom: 10px;'></div>"
                                }]
                            });
                        }
                        if (win.isVisible()) {
                            win.close(this);
                            win = undefined;
                        }
                        else {
                            win.show(this);
                        }
                        $("#uploadify").uploadify({
                            'uploader': '/Scripts/jquery.uploadify-v2.1.0/uploadify.swf',
                            'script': '/Course/SaveCourseImg',
                            'cancelImg': '/Scripts/jquery.uploadify-v2.1.0/cancel.png',
                            'folder': 'UploadFile',
                            'queueID': 'fileQueue',
                            'fileExt': '*.gif;*.jpg;*.png',
                            'fileDesc': '*.gif;*.jpg;*.png',
                            //'buttonImg': '/img.gigade100.com/product/nopic_150.jpg',
                            'buttonText': SELECT_FILE + '...',//選擇文件
                            'auto': false,
                            'multi': true,
                            //'scriptData': { ASPSESSID: window.parent.GethfAspSessID(), AUTHID: window.parent.GethfAuth() },
                            'onComplete': function (event, queueId, fileObj, response, data) {
                                var resText = eval("(" + response + ")");
                                if (resText.success == "True") {
                                    var index = imgstore.data.items.length + 1;
                                    imgstore.insert(index, { id: index, picture_name: resText.fileName, picture_type: resText.Type, course_id: 0 });
                                    //win.hide();
                                }
                                else {
                                    //錯誤處理
                                    Ext.Msg.alert(MESSAGE, UPLOADING_FILED);//消息,上傳失敗
                                }
                            }
                        });

                    }
                }, imgView]
            }]
        }]
    }, {
        flex: 3.0,
        title: DETAILS,//細項
        xtype: 'gridpanel',
        id: 'detailist',
        autoScroll: true,
        frame: false,
        store: courseDetailStore,
        columns: [
            { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 46, align: 'center' },//序號
            { header: CURRICULUM_NAME, dataIndex: 'Course_Detail_Name', align: 'left', width: 220, menuDisabled: true, sortable: false },//課程名稱
            { header: ATTEND_CURRICULUM_PLACE, dataIndex: 'Address', align: 'left', width: 220, menuDisabled: true, sortable: false },//上課地點
            { header: ONCE_CURRICULUM_BEGIN_DATE, dataIndex: 'Start_Date',/* renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s'),*/ align: 'left', width: 200, menuDisabled: true, sortable: false, renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s') },//單節課程開始時間
            { header: ONCE_CURRICULUM_END_DATE, dataIndex: 'End_Date', renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s'), field: 'datetimefield', align: 'left', width: 220, menuDisabled: true, sortable: false },//單節課程結束時間
            { header: PEOPLE_COUNT, dataIndex: 'P_Number', align: 'left', width: 60, menuDisabled: true, sortable: false },//人數
        { header: BUY_TICKET_PEOPLE_COUNT, dataIndex: 'P_NumberReality', align: 'left', width: 100, menuDisabled: true, sortable: false, flex: 1 }],//購票人數
        tbar: [
            {
                text: THIS_INSERT, id: 'add', handler: function () {//新增
                    detailAdd(courseDetailStore);
                }
            },
            { text: THIS_EDIT, id: 'edit', handler: detailEdit, disabled: true },//修改
            {
                text: THIS_DELETE, id: 'delete', disabled: true, handler: function () {//刪除
                    detailDelete(courseDetailStore);
                }
            }],
        selModel: cbm,
        listeners: {
            itemclick: function (grid, record) {
                //courseTicketStore.load({//edit by wwei0216w 2015/6/2 暫不需要關於票據的代碼,將其去掉(後臺代碼仍有關於屬性)
                //    params: {
                //        course_detail_id: record.data.Course_Detail_Id
                //    }
                //})
            }
        }
    }, {
        flex: 2.4,
        title: TICKET,//TICKET
        xtype: 'gridpanel',
        id: 'ticketlist',
        autoScroll: true,
        hidden: true,
        frame: false,
        store: courseTicketStore,
        columns: [
        { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 46, align: 'center' },//序號
        { header: CODE, dataIndex: 'Ticket_Code', align: 'left', width: 150, menuDisabled: true, sortable: false },//代碼
            { header: USER_NAME, dataIndex: 'User_Name', align: 'left', width: 120, menuDisabled: true, sortable: false },//用戶名
            { header: CREATE_DATE, dataIndex: 'Create_Date', renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s'), align: 'left', width: 200, menuDisabled: true, sortable: false },//創建時間
        { header: TICKET_STATE, dataIndex: 'FlagName', align: 'left', width: 100, menuDisabled: true, sortable: false, flex: 1 }]//票券狀態
    }],
    bbar: [{
        text: THIS_SAVE,//保存
        id: 'btn_save',
        iconCls: 'ui-icon ui-icon-checked',
        handler: Save
    }, {
        text: THIS_RESET,//重置
        id: 'btn_reset',
        iconCls: 'ui-icon ui-icon-reset',
        handler: function () { LoadDetail(currentRecord); }
    }, {
        text: THIS_CANCEL,//取消
        id: 'btn_cancel',
        iconCls: 'ui-icon ui-icon-cancel',
        handler: function () {
            Ext.getCmp('west-region-container').setDisabled(false);
        }
    }]
})


function detailAdd(store) {

    var detailAddFrm = Ext.create('Ext.form.Panel', {
        id: 'detailAddFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 45,
        defaults: { msgTarget: "side", labelWidth: 60 },
        items: [{
            xtype: 'textfield',
            fieldLabel: CURRICULUM_NAME,//課程名稱
            id: 'dadd_course_detail_name',
            name: 'Course_Detail_Name',
            allowBlank: false,
            width: 310
        }, {
            xtype: 'textfield',
            fieldLabel: ATTEND_CURRICULUM_PLACE,//上課地點
            id: 'dadd_course_address',
            name: 'Address',
            allowBlank: false,
            width: 310
        }, {
            xtype: 'datetimefield',
            format: 'Y-m-d H:i:s',
            time:{hour:'00',sec:'00',min:'00'},
            fieldLabel: BEGIN_CURRICULUM_TIME,//上課時間
            id: 'dadd_start_date',
            name: 'Start_Date',
            allowBlank: false,
            editable: false,
            celleditable: false,
            //vtype: 'daterange',
            //endDateField: 'dadd_end_date',
            listeners: {//change by shiwei0620j 20151217,將時間控件改為可以選擇時分秒，開始時間時分秒默認為00:00:00,結束時間時分秒默認為23:59:59，當選擇的開始時間大於結束時間，結束時間在開始時間月份加1，當選擇的結束時間大於開始時間，開始時間在結束時間月份加1;
                select: function (a, b, c) {
                    var start = Ext.getCmp("dadd_start_date");
                    var end = Ext.getCmp("dadd_end_date");
                    var start_date = start.getValue();
                    if (end.getValue() == "" || end.getValue() == null) {
                        Ext.getCmp('dadd_end_date').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                    }
                    else if (end.getValue() < start.getValue()) {
                        Ext.getCmp('dadd_end_date').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                    }
                }
            },
            width: 230
        }, {
            xtype: 'datetimefield',
            format: 'Y-m-d H:i:s',
            time: { hour: '23', sec: '59', min: '59' },
            fieldLabel: END_CURRICULUM_TIME,//下課時間
            id: 'dadd_end_date',
            name: 'End_Date',
            allowBlank: false,
            editable: false,
            celleditable: false,
            //vtype: 'daterange',
            //endDateField: 'dadd_start_date',
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("dadd_start_date");
                    var end = Ext.getCmp("dadd_end_date");
                    var end_date = end.getValue();
                    if (start.getValue() == "" || start.getValue() == null) {
                        Ext.getCmp('dadd_start_date').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                    }
                    if (end.getValue() < start.getValue()) {
                        Ext.getCmp('dadd_start_date').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                    }
                }
            },
            width: 230
        }, {
            xtype: 'numberfield',
            fieldLabel: PEOPLE_COUNT,//人數
            id: 'dadd_p_number',
            name: 'P_Number',
            allowBlank: false,
            minValue: 1,
            width: 160
        }],
        buttons: [{
            text: THIS_RESET,//重置
            handler: function () {
                this.up('form').getForm().reset();
            }
        }, {
            text: AFFIRM,//確定
            handler: function () {
                if (Ext.getCmp('dadd_start_date').getValue().getTime() >= Ext.getCmp('dadd_end_date').getValue().getTime()) {
                    Ext.Msg.alert(INFORMATION, BEGIN_DATE_MUST_LT_END_DATE);//開始時間必須小於結束時間
                    return;
                }
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    store.add({
                        Course_Detail_Id: 0,
                        Course_Id: currentRecord.data.Course_Id,
                        Course_Detail_Name: Ext.getCmp('dadd_course_detail_name').getValue(),
                        Address: '',
                        Start_Date: '',
                        End_Date: '',
                        P_Number: '',
                        P_NumberReality: 0,
                        Address: Ext.getCmp('dadd_course_address').getValue(),
                        Start_Date: Ext.getCmp('dadd_start_date').getValue(),
                        End_Date: Ext.getCmp('dadd_end_date').getValue(),
                        P_Number: Ext.getCmp('dadd_p_number').getValue()
                    });
                    detailAddWin.destroy();
                }
            }
        }]
    })

    var detailAddWin = Ext.create('Ext.window.Window', {
        title: CREAT_NEW_CURRICULUM_DETAILS,//新增課程詳情
        width: 400,
        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [detailAddFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                detailAddFrm.getForm().reset();
            }
        }
    })

    detailAddWin.show();

}

function detailEdit(row, store) {

    var sms = Ext.getCmp("detailist").getSelectionModel().getSelection();
    if (sms.length == 0) {
        Ext.Msg.alert(INFORMATION, NO_SELECTION);
    }
    else if (sms.length > 1) {
        Ext.Msg.alert(INFORMATION, ONE_SELECTION);
    } else if (sms.length == 1) {
        if (sms[0].data.P_NumberReality == 0) {
            EditFunction(sms[0], courseDetailStore);
        } else {
            Ext.Msg.alert(INFORMATION, SOMEBODY_BUY_TICKET_CANNOT_EDIT)//購票人數大於0，不能進行修改
        }
    }

}

EditFunction = function (row, store) {

    var detailEditFrm = Ext.create('Ext.form.Panel', {
        id: 'detailEditFrm',
        frame: true,
        plain: true,
        layout: 'anchor',
        labelWidth: 45,
        defaults: { msgTarget: "side", labelWidth: 60 },
        items: [{
            xtype: 'textfield',
            fieldLabel: CURRICULUM_NAME,//課程名稱
            id: 'dedit_course_detail_name',
            name: 'Course_Detail_Name',
            allowBlank: false,
            width: 310
        }, {
            xtype: 'textfield',
            fieldLabel: ATTEND_CURRICULUM_PLACE,//上課地點
            id: 'dedit_course_address',
            name: 'Address',
            allowBlank: false,
            width: 310
        }, {
            xtype: 'datetimefield',
            format: 'Y-m-d H:i:s',
            time:{hour:'00',min:'00',sec:'00'},
            fieldLabel: BEGIN_CURRICULUM_TIME,//上課時間
            id: 'dedit_start_date',
            name: 'Start_Date',
            allowBlank: false,
            listeners: {//change by shiwei0620j 20151217,將時間控件改為可以選擇時分秒，開始時間時分秒默認為00:00:00,結束時間時分秒默認為23:59:59，當選擇的開始時間大於結束時間，結束時間在開始時間月份加1，當選擇的結束時間大於開始時間，開始時間在結束時間月份加1;
                select: function (a, b, c) {
                    var start = Ext.getCmp("dedit_start_date");
                    var end = Ext.getCmp("dedit_end_date");
                    var start_date = start.getValue();
                    if (end.getValue() == "" || end.getValue() == null) {
                        Ext.getCmp('dedit_end_date').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                    }
                    else if (end.getValue() < start.getValue()) {
                        Ext.getCmp('dedit_end_date').setValue(new Date(start_date.getFullYear(), start_date.getMonth() + 1, start_date.getDate(), 23, 59, 59));
                    }
                }
            },
            width: 230
        }, {
            xtype: 'datetimefield',
            format: 'Y-m-d H:i:s',
            time: { hour: '23', min: '59', sec: '59' },
            fieldLabel: END_CURRICULUM_TIME,//下課時間
            id: 'dedit_end_date',
            name: 'End_Date',
            allowBlank: false,
            listeners: {
                select: function (a, b, c) {
                    var start = Ext.getCmp("dedit_start_date");
                    var end = Ext.getCmp("dedit_end_date");
                    var end_date = end.getValue();
                    if (start.getValue() == "" || start.getValue() == null) {
                        Ext.getCmp('dedit_start_date').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                    }
                    if (end.getValue() < start.getValue()) {
                        Ext.getCmp('dedit_start_date').setValue(new Date(end_date.getFullYear(), end_date.getMonth() - 1, end_date.getDate()));
                    }
                }
            },
            width: 230
        }, {
            xtype: 'numberfield',
            fieldLabel: PEOPLE_COUNT,//人數
            id: 'dedit_p_number',
            name: 'P_Number',
            allowBlank: false,
            width: 160
        }],
        buttons: [{
            text: THIS_RESET,//重置
            handler: function () {
                detailEditFrm.getForm().loadRecord(row);
            }
        }, {
            text: AFFIRM,//確定
            handler: function () {
                if (Ext.getCmp('dedit_start_date').getValue().getTime() >= Ext.getCmp('dedit_end_date').getValue().getTime()) {
                    Ext.Msg.alert(INFORMATION, BEGIN_DATE_MUST_LT_END_DATE);//開始時間必須小於結束時間
                    return;
                }
                var form = this.up('form').getForm();
                if (form.isValid()) {
                    row.set('Address', Ext.getCmp('dedit_course_address').getValue());
                    row.set('Course_Detail_Name', Ext.getCmp('dedit_course_detail_name').getValue());
                    row.set('Start_Date', Ext.getCmp('dedit_start_date').getValue());
                    row.set('End_Date', Ext.getCmp('dedit_end_date').getValue());
                    row.set('P_Number', Ext.getCmp('dedit_p_number').getValue());
                    detailEditWin.destroy();
                }
            }
        }]
    })


    var detailEditWin = Ext.create('Ext.window.Window', {
        title: EDIT_CURRICULUM_DETAILS,//修改課程詳情
        width: 400,
        height: document.documentElement.clientHeight * 260 / 783,
        layout: 'fit',
        items: [detailEditFrm],
        closeAction: 'destroy',
        modal: true,
        resizable: false,
        labelWidth: 60,
        bodyStyle: 'padding:5px 5px 5px 5px',
        listeners: {
            'show': function () {
                detailEditFrm.getForm().loadRecord(row);
            }
        }
    });

    detailEditWin.show();
}

function detailDelete(store) {
    var row = Ext.getCmp("detailist").getSelectionModel().getSelection();
    if (row[0].data.P_NumberReality != 0) {
        Ext.Msg.alert(INFORMATION, SOMEBODY_BUY_TICKET_CANNOT_DELETE)//購票人數大於0，不能刪除
    }
    else {
        Ext.Msg.confirm(CONFIRM, Ext.String.format(DELETE_INFO, row.length), function (btn) {
            if (btn == 'yes') {
                store.remove(row);
            }
        });
    }
}




function LoadDetail(record) {

    Ext.getCmp("course_name").setValue(record.data.Course_Name);
    Ext.getCmp("tel").setValue(record.data.Tel);
    Ext.getCmp("start_date").setRawValue(Ext.Date.format(record.data.Start_Date, 'Y-m-d H:i:s'));
    Ext.getCmp("end_date").setRawValue(Ext.Date.format(record.data.End_Date, 'Y-m-d H:i:s'));
    Ext.getCmp("msg").setValue(record.data.Msg);
    Ext.getCmp("mail_content").setValue(record.data.Mail_Content);

    if (record.data.Send_Msg == 0) {
        Ext.getCmp("send_msg").setValue(true);
    } else {
        Ext.getCmp("send_msg").setValue(false);
    }

    if (record.data.Send_Mail == 0) {
        Ext.getCmp("send_mail").setValue(true)
    } else {
        Ext.getCmp("send_mail").setValue(false);
    }

    if (record.data.Source == 0) {
        Ext.getCmp("zb").setValue(true);
    } else {
        Ext.getCmp("hzs").setValue(true);
    }
    if (record.data.Ticket_Type == 0) {
        Ext.getCmp("xn").setValue(true);
    } else {
        Ext.getCmp("st").setValue(true);
    }
    courseDetailStore.load({
        params: {
            Course_Id: record.data.Course_Id
        }
    });
    //imgstore.removeAll();
    imgstore.load({
        params: {
            course_Id: record.data.Course_Id
        }
    });
}


function Search() {
    Ext.getCmp('grid_course_id').setValue(Ext.getCmp('grid_course_id').getValue().replace(/\s+/g, ','));
    if (!Ext.getCmp('grid_course_id').isValid()) return;
    courseListStore.removeAll();
    courseListStore.loadPage(1);
}


function Save() {

    if (!Ext.getCmp('course_name').isValid() || !Ext.getCmp('tel').isValid()) return;

    //if (!Ext.getCmp('start_date').isValid() || !Ext.getCmp('end_date').isValid()) return;

    if (Ext.getCmp('start_date').getValue().getTime() >= Ext.getCmp('end_date').getValue().getTime()) {
        Ext.Msg.alert(INFORMATION, BEGIN_DATE_MUST_LT_END_DATE);//開始時間必須小於結束時間
        return;
    }
    if (imgstore.data.items.length > 10) {
        Ext.Msg.alert(INFORMATION, IMAGE_CANNOT_EXCEED_10);//購票人數大於0，不能刪除
        return;
    }


    if (courseDetailStore.data.length <= 0) {
        Ext.Msg.alert(INFORMATION, PLEASE_WRITE_DETAILS_DATA);//請將細項資料填寫完整
        return;
    }

    var myMask = new Ext.LoadMask(Ext.getBody(), {
        msg: 'Loading...'
    });
    myMask.show();

    var course = {};
    course.Course_Id = currentRecord.data.Course_Id == null ? 0 : currentRecord.data.Course_Id;
    course.Course_Name = Ext.getCmp('course_name').getValue();
    course.Tel = Ext.getCmp('tel').getValue();
    course.Start_Date = Ext.getCmp('start_date').getValue();
    course.End_Date = Ext.getCmp('end_date').getValue();
    course.Msg = Ext.getCmp('msg').getValue();
    course.Mail_Content = Ext.getCmp('mail_content').getValue();
    course.Source = Ext.getCmp('source').getValue().source;
    course.Ticket_Type = Ext.getCmp('ticket_type').getValue().ticket_type;

    if (Ext.getCmp('send_msg').getValue()) {
        course.Send_Msg = 0;
    } else {
        course.Send_Msg = 1;
    }

    if (Ext.getCmp('send_mail').getValue()) {
        course.Send_Mail = 0;
    } else {
        course.Send_Mail = 1;
    }

    var courseDetail = [];

    for (var i = 0, j = courseDetailStore.data.length ; i < j; i++) {
        var record = courseDetailStore.data.items[i];

        courseDetail.push({

            'Course_Detail_Id': record.get("Course_Detail_Id") == 0 ? 0 : record.get("Course_Detail_Id"),
            'Course_Id': currentRecord.data.Course_Id == null ? 0 : currentRecord.data.Course_Id,
            'Course_Detail_Name': record.get("Course_Detail_Name"),
            'Address': record.get("Address"),
            'Start_Date': Ext.Date.format(new Date(record.get("Start_Date")), 'Y-m-d H:i:s'),
            'End_Date': Ext.Date.format(new Date(record.get("End_Date")), 'Y-m-d H:i:s'),
            'P_Number': record.get("P_Number"),
            'P_NumberReality': 0

        });
    }

    var imgDetail = [];
    for (var i = 0, j = imgstore.data.length ; i < j; i++) {
        var record = imgstore.data.items[i];
        imgDetail.push({
            'id': record.get("id") == 0 ? 0 : record.get("id"),
            'course_id': record.get("course_id") == null ? 0 : record.get("course_id"),
            'picture_name': record.get("picture_name"),
            'picture_type': record.get("picture_type"),
            'picture_status': record.get("picture_status"),
            'picture_sort': record.get("picture_sort")
        });
    }

    var imgStores = Ext.encode(imgDetail);

    var prodcourseDetail = Ext.encode(courseDetail);

    Ext.Ajax.request({
        url: '/Course/CurriculSave',
        params: {
            courseStr: Ext.encode(course),
            courseDetailStrs: prodcourseDetail,
            imgStore: imgStores
        },
        timeout: 360000,
        success: function (response) {
            var res = Ext.decode(response.responseText);
            if (res.success) {
                Ext.Msg.alert(INFORMATION, SUCCESS);
                myMask.hide();
                Ext.getCmp('west-region-container').setDisabled(false);
                Search();
            }
            else {
                Ext.Msg.alert(INFORMATION, SAVE_FILED);//保存失敗
                myMask.hide();
            }

        },
        failure: function (response, opts) {
            if (response.timedout) {
                Ext.Msg.alert(INFORMATION, TIME_OUT);
            }
            myMask.hide();
        }
    });
}
