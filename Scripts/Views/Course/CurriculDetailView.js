
// CourseDetailList 詳情
Ext.define('GIGADE.COURSEDETAIL', {
    extend: 'Ext.data.Model',
    fields: [
        { name: 'Course_Detail_Id', type: 'int' },
        { name: 'Course_Id', type: 'int' },
        { name: 'Course_Detail_Name', type: 'string' },
        { name: 'Address', type: 'string' },
        { name: 'Start_Date', type: 'string' },
        { name: 'End_Date', type: 'string' },
        { name: 'P_Number', type: 'int' },
        { name: 'P_NumberReality', type: 'int' }]
});

Ext.onReady(function () {
    var course_id = document.getElementById('course_id').value;
    var courseDetailStore = Ext.create('Ext.data.Store', {
        autoLoad: true,
        model: 'GIGADE.COURSEDETAIL',
        proxy: {
            type: 'ajax',
            url: '/Course/GetCurriculDetail?course_id=' + course_id,
            actionMethods: 'post',
            reader: {
                type: 'json'
            }
        }
    });

    var top = Ext.create('Ext.form.Panel', {
        id: 'top',
        border: false,
        autoScroll: true,
        margin: '3 0 0 8',
        defaults: {
            labelWidth: 80,
            padding: '8 0 0 0'
        },
        items: [{
            xtype: 'container',
            layout: 'hbox',
            id: 'part1',
            defaults: { width: 320, labelWidth: 30 },
            items: [{
                xtype: 'container',
                id: 'part1-1',
                defaults: { width: 320 },
                items: [{
                    xtype: 'textfield',
                    id: 'course_name',//課程名稱
                    fieldLabel: CURRICULUM_NAME,
                    readOnly: true,
                    allowBlank: false
                }, {
                    xtype: 'textfield',
                    id: 'tel',//聯繫電話
                    allowBlank: false,
                    readOnly: true,
                    fieldLabel: CONN_TEL
                }]
            }, {
                xtype: 'container',
                margin: '0 0 0 20',
                id: 'part1-2',
                defaults: { width: 260 },
                items: [{
                    xtype: 'datetimefield',
                    format: 'Y-m-d H:i:s',
                    id: 'start_date',
                    allowBlank: false,
                    readOnly: true,
                    fieldLabel: BEGIN_DATE//開始時間
                }, {
                    xtype: 'datetimefield',
                    format: 'Y-m-d H:i:s',
                    id: 'end_date',
                    allowBlank: false,
                    readOnly: true,
                    fieldLabel: END_DATE//結束時間
                }]
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
                    readOnly: true,
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
                    readOnly: true,
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
                    readOnly: true,
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
                    readOnly: true,
                    height: 50,
                    id: 'mail_content'
                }]
            }]
        }, {
            xtype: 'container',
            layout: 'column',
            id: 'part3',
            items: [{
                xtype: 'radiogroup',
                fieldLabel: CURRICULUM_FROM,//課程來源
                width: 360,
                id: 'source',
                columns: 2,
                vertical: true,
                readOnly: true,
                defaults: { name: 'source' },
                items: [
                    { boxLabel: PERSONAL_TRANSACT, id: 'zb', inputValue: 0, checked: true, readOnly: true },//自辦
                    { boxLabel: COOPERATION_BUSINESS, id: 'hzs', inputValue: 1, readOnly: true }]//合作商
            }, {
                xtype: 'radiogroup',
                fieldLabel: TICKET_TYPE,//票劵類型
                width: 320,
                id: 'ticket_type',
                columns: 2,
                vertical: true,
                readOnly: true,
                defaults: { name: 'ticket_type' },
                items: [
                    { boxLabel: VIRTUAL, id: 'xn', inputValue: 0, checked: true, readOnly: true },//虛擬
                    { boxLabel: SUBSTANCE, id: 'st', inputValue: 1, readOnly: true }]//實體
            }]
        }]
    })

    var bottom = Ext.create('Ext.grid.Panel', {
        id: 'bottom',
        autoScroll: true,
        frame: false,
        store: courseDetailStore,
        columns: [
            { header: SEQUENCE_NUMBER, xtype: 'rownumberer', width: 46, align: 'center' },//序號
            { header: CURRICULUM_NAME, dataIndex: 'Course_Detail_Name', align: 'left', width: 150, menuDisabled: true, sortable: false },//課程名稱
            { header: ATTEND_CURRICULUM_PLACE, dataIndex: 'Address', align: 'left', width: 150, menuDisabled: true, sortable: false },//上課地點
            { header: ONCE_CURRICULUM_BEGIN_DATE, dataIndex: 'Start_Date'/*, renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s')*/, align: 'left', width: 200, menuDisabled: true, sortable: false },//單節課程開始時間
            { header: ONCE_CURRICULUM_END_DATE, dataIndex: 'End_Date', renderer: Ext.util.Format.dateRenderer('Y-m-d H:i:s'), field: 'datetimefield', align: 'left', width: 220, menuDisabled: true, sortable: false },//單節課程結束時間
            { header: PEOPLE_COUNT, dataIndex: 'P_Number', align: 'left', width: 60, menuDisabled: true, sortable: false },//人數
            { header: BUY_TICKET_PEOPLE_COUNT, dataIndex: 'P_NumberReality', align: 'left', width: 100, menuDisabled: true, sortable: false }]//購票人數
    })


    Ext.create('Ext.Viewport', {
        id: "index",
        autoScroll: true,
        width: document.documentElement.clientWidth,
        height: document.documentElement.clientHeight,
        layout: 'anchor',
        items: [top, bottom],
        renderTo: Ext.getBody(),
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
        }
    });


    Ext.Ajax.request({
        url: '/Course/GetCurricul',
        params: {
            course_id: course_id
        },
        success: function (response) {
            var record = { data: Ext.decode(response.responseText)[0] };
            Ext.getCmp("course_name").setValue(record.data.Course_Name);
            Ext.getCmp("tel").setValue(record.data.Tel);
            Ext.getCmp("start_date").setRawValue(record.data.Start_Date);
            Ext.getCmp("end_date").setRawValue(record.data.End_Date);
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
        },
        failure: function (response, opts) {
            if (response.timedout) {
                Ext.Msg.alert(INFORMATION, TIME_OUT);
            }
        }
    });
});





