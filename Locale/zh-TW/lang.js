IntegerValidate = "此欄位必須為整數";
YES = "是";
NO = "否";
CLOSEFORM = "關閉";

Ext.onReady(function () {
    if (Ext.tab.Panel) {
        WELCOME = "歡迎您";
        EDIT_INFO = "修改信息";
        LOGOUT = "登出";
    }
    if (Ext.panel.Panel) {
        CURRENT_TIME = "當前時間";
    }
    if (Ext.Msg.alert) {
        INFORMATION = "提示信息";
        NO_SELECTION = "未選中任何行!";
        NO_DATA = "未選中任何數據!";
        ONE_SELECTION = "只能选择一行!";
        SUCCESS = "操作成功";
        FAILURE = "操作失敗";
        DATA_LOAD_FAILURE = "數據加載失敗";
        DATE_LIMIT = "日期區間必須在一月之內!";
        SEARCH_LIMIT = "請輸入查詢條件！";
        DATA_TIP = "結束時間不能小於開始時間！";
    }

    if (Ext.Msg.confirm) {
        CONFIRM = "確認";
        DELETE_INFO = "刪除選中 {0} 條數據？";
        IS_CONTNUES = '上次操作數據未保存，是否繼續？<br/> "Yes" : 繼續上次操作<br/> "No" : 刪除臨時數據';
        IS_CLOSEFORM = "是否確定關閉窗口?";
        CANCEL_INFO = "取消送審選中{0}條數據"
    }

    if (Ext.button.Button) {
        ADD = "新增";
        EDIT = "編輯";
        REMOVE = "移除";
        SAVE = "保存";
        SUBMIT = "提交";
        RESET = "重置";
        SURE = "確定";
        IMPORT = "匯入";
        CANCEL = "取消";
        RETURN = "返回";
        SEARCH = "查詢";
        COPY = "複製";
        CLEAR = "清空";
    }

    if (Ext.grid.Panel) {
        NUMBER = "序號";
    }

    if (Ext.toolbar.Paging) {
        NOW_DISPLAY_RECORD = "當前顯示記錄";
        TOTAL = "共計";
        NOTHING_DISPLAY = "沒有記錄可以顯示";
    }




});
