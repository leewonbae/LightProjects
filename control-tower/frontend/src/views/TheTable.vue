<template>
  <div>
    <div class="group-list">
      <h2>Table Uploader</h2>

      <div class="page-result">
        <div class="panel">
          <div class="panel-body">
            <div class="panel-table">
              <h3>업데이트 할 테이블을 입력해주세요 (xlsx 확장자만 선택가능)</h3>
              <table class="table table-bordered table-hover dataTable">
                <thead class="table-align">
                  <th>타겟 DB</th>
                  <th>업로드 테이블</th>
                </thead>
                <tbody>
                  <tr>
                    <td>
                      <select v-model="selectDbName" class="form-control">
                        <option v-for="db in dbList" :key="db.id" :value="db.name">
                          {{ db.name }} ({{ db.branch }})
                        </option>
                      </select>
                    </td>
                    <td>
                      <form enctype="multipart/form-data">
                        <input
                          id="selectFile"
                          name="selectFile"
                          multiple="multiple"
                          type="file"
                          accept=".xlsx"
                          :disabled="isSuccessUploadingDataFile == true"
                          @change="readTableFile" />
                      </form>
                    </td>
                  </tr>
                </tbody>
              </table>
              <div>
                <span v-if="isSuccessUploadingDataFile == false">
                  <button type="submit" @click="uploadTableFile()">데이터 파일 업로드</button>
                </span>
                <span v-else>
                  <span v-if="verifyResultInfoList == null">
                    <button colspan="2" type="submit" @click="startTableVerify()">검증 시작</button>
                    <button class="button orange" type="submit" @click="clearInputFileInfo()">
                      초기화
                    </button>
                  </span>
                </span>
              </div>
              <div v-if="tableAdjustStep == 1" class="group-list">
                <h3>검증 결과</h3>
                <table class="table table-bordered table-hover">
                  <thead class="table-align">
                    <tr>
                      <th>시트 이름</th>
                      <th>유효 컬럼 수</th>
                      <th>마지막 확인 로우 번호(이름로우+ 타입로우+ 데이터로우)</th>
                      <th>검증 결과</th>
                      <th>에러 내용</th>
                      <th>테이블 업로드 대상 선택</th>
                    </tr>
                  </thead>
                  <tbody class="table-align">
                    <tr v-for="(resultInfo, index) in verifyResultInfoList" :key="index">
                      <td>{{ resultInfo.sheetName }}</td>
                      <td>{{ resultInfo.validColumnCount }}</td>
                      <td>{{ resultInfo.validRowCount }}</td>
                      <td v-if="resultInfo.isVerifyFail == true">실패</td>
                      <td v-if="resultInfo.isVerifyFail == false">
                        <span v-if="resultInfo.isNeedToCheckFile == true">
                          성공(로우 수 확인 필요)
                        </span>
                        <span v-else>성공</span>
                      </td>
                      <td>
                        <button
                          v-if="resultInfo.errorList.length != 0"
                          type="submit"
                          @click="openErrorModal(resultInfo.errorList)">
                          에러 내용 확인
                        </button>
                      </td>
                      <td>
                        <input
                          v-model="resultInfo.selected"
                          type="checkbox"
                          @click="clickTableSelectCheckBox(index)" />
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <div v-if="tableAdjustStep == 1 && verifyResultInfoList != null">
                <h3>실행 여부</h3>
                <button
                  v-if="errorCount == 0 && selectTableCount != 0"
                  type="submit"
                  @click="updateTable()">
                  <span v-if="selectTableCount == maxTableCount">테이블 전체 업로드 시작</span>
                  <span v-else>
                    선택된 테이블만 업로드 시작 ( 선택된 카운트 : {{ selectTableCount }} )
                  </span>
                </button>
                <button class="button orange" type="submit" @click="reqClearFile()">취소</button>
              </div>
            </div>
            <div v-if="tableAdjustStep == 2">
              <h3>서버 재로딩</h3>
              <div class="panel-table">
                <table class="table table-bordered table-hover dataTable">
                  <thead class="table-align">
                    <th>업데이트 된 테이블 이름</th>
                  </thead>
                  <tbody class="table-align">
                    <tr v-for="tableName in updateTableNameList" :key="tableName">
                      <td>{{ tableName }}</td>
                    </tr>
                  </tbody>
                </table>
                <table class="table table-bordered table-hover dataTable">
                  <thead class="table-align">
                    <tr>
                      <th>서버이름</th>
                      <th>브랜치</th>
                      <th>DB명</th>
                      <th>관리</th>
                    </tr>
                  </thead>
                  <tbody class="table-align">
                    <tr v-for="server in serverList" :key="server.id" :colspan="4">
                      <td v-if="server.database == selectDbName">{{ server.name }}</td>
                      <td v-if="server.database == selectDbName">{{ server.branch }}</td>
                      <td v-if="server.database == selectDbName">
                        {{ server.database }}
                      </td>
                      <td v-if="server.database == selectDbName">
                        <button type="button" class="button orange" @click="reloadTable(server)">
                          테이블 재로딩
                        </button>
                      </td>
                    </tr>
                  </tbody>
                </table>
              </div>
              <button class="button orange" type="submit" @click="clearInputFileInfo()">
                완료
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>

  <!-- 서버 생성 모달 START -->
  <modal-view :open-modal="showErrorModal" :title="errorModalTitle" @close-modal="closeErrorModal">
    <template #default>
      <div class="modals-body modals-vertical-body">
        <table class="table">
          <thead class="table-align">
            <tr>
              <th>Id</th>
              <th>에러 내용</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(errorInfo, index) in errorInfoList" :key="index">
              <td>{{ index + 1 }}</td>
              <td>{{ errorInfo }}</td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <template #footer>
      <button type="button" class="button button-close" @click="closeErrorModal">닫기</button>
    </template>
  </modal-view>
  <!-- 서버 생성 모달 END -->
</template>

<script>
import http from "@/utils/http-common";
import axios from "axios";
import ModalView from "@/components/ModalView";

export default {
  name: "TheTable",
  components: {
    ModalView,
  },
  data() {
    return {
      server_url: "http://192.168.0.10:7008",
      dbList: [],
      serverList: [],
      uploadFileInfoList: null,
      isSuccessUploadingDataFile: false,
      selectDbName: "default",
      updateTableNameList: null,
      verifyResultInfoList: null,
      errorCount: 0,
      selectTableCount: 0,
      maxTableCount: 0,
      selectedSheetNameList: [],
      //
      errorModalTitle: "에러 목록",
      showErrorModal: false,
      errorInfoList: null,

      //
      tableAdjustStep: 0,
      tableAdjustMaxStep: 3,
    };
  },
  async mounted() {
    await this.loadDbList();
  },
  methods: {
    setNextStep() {
      this.tableAdjustStep = (this.tableAdjustStep + 1) % this.tableAdjustMaxStep;
    },
    async loadDbList() {
      http
        .get("/all")
        .then(res => {
          if (res.data) {
            this.dbList = res.data.dbList;
            this.serverList = res.data.serverList;
          }
        })
        .catch(err => {
          console.log(err);
        });
    },
    async reloadTable(server) {
      if (!confirm(`서버 [${server.name}]의 테이블을 재로딩하시겠습니까?`)) {
        return;
      }
      http.get(`http://${server.ip}:${server.port}/system/reload-table`).then(res => {
        if (res.data) {
          alert("서버 테이블 재로딩 완료 ");
        }
      });
    },
    openErrorModal(errorInfo) {
      this.showErrorModal = true;
      this.errorInfoList = errorInfo;
    },
    closeErrorModal() {
      this.showErrorModal = false;
    },
    readTableFile($event) {
      this.uploadFileInfoList = $event.target.files;
    },
    clickTableSelectCheckBox(index) {
      this.verifyResultInfoList[index].selected = !this.verifyResultInfoList[index].selected;
      if (this.verifyResultInfoList[index].selected == true) {
        this.selectTableCount++;
      } else {
        this.selectTableCount--;
      }
      console.log("select Count ==== " + this.selectTableCount);
    },
    clearInputFileInfo() {
      document.getElementById("selectFile").value = "";
      this.uploadFileInfoList = null;
      this.isSuccessUploadingDataFile = false;
      this.verifyResultInfoList = null;
      this.errorCount = 0;
      this.selectTableCount = 0;
      this.maxTableCount = 0;
      this.selectedSheetNameList = [];
      this.tableAdjustStep = 0;
    },

    async uploadTableFile() {
      if (this.uploadFileInfoList == null) {
        alert("파일이 선택되지 않았습니다.");
        return;
      }

      const formData = new FormData();
      for (let index = 0; index < this.uploadFileInfoList.length; ++index) {
        formData.append("files", this.uploadFileInfoList[index]);
      }

      await axios
        .post(`${this.server_url}/action/file`, formData, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        })
        .then(res => {
          this.isSuccessUploadingDataFile = Boolean(res.data);
          if (this.isSuccessUploadingDataFile) {
            alert("데이터 테이블 업로드 성공");
          } else {
            alert("데이터 테이블 업로드 실패");
          }
        })
        .catch(e => {
          console.log(e);
        });
    },
    async startTableVerify() {
      await axios
        .get(`${this.server_url}/action/start_table_verify`)
        .then(res => {
          this.verifyResultInfoList = res.data.result;
          this.verifyResultInfoList.forEach(element => {
            if (element.errorList.length != 0) {
              this.errorCount++;
            }
            element.selected = true;
            this.selectTableCount++;
          });
          this.maxTableCount = this.selectTableCount;
          alert("데이터 검증 완료");
          this.setNextStep();
        })
        .catch(e => {
          console.log(e);
        });
    },
    async reqClearFile() {
      await axios
        .get(`${this.server_url}/action/clear_file`)
        .then(res => {
          Boolean(res.data);
          this.clearInputFileInfo();
          alert("작업을 취소 했습니다.");
        })
        .catch(e => {
          console.log(e);
        });
    },
    async updateTable() {
      if (this.selectTableCount != this.maxTableCount) {
        this.verifyResultInfoList.forEach(i => {
          if (i.selected == true) {
            console.log("select" + i.sheetName);
            this.selectedSheetNameList.push(i.sheetName);
          }
        });
      }

      if (this.errorCount == 0) {
        if (confirm(`${this.selectDbName} DB에 반영을 진행하시겠습니까?`) == false) {
          return;
        }
      }

      await axios
        .post(
          `${this.server_url}/action/update-table`,
          {
            branchName: this.selectDbName,
            selectedTableNameList: this.selectedSheetNameList,
          },
          {
            headers: {
              "Content-Type": "application/json",
            },
          },
        )
        .then(res => {
          if (res.data.result.length != 0) {
            this.updateTableNameList = res.data.result;
            alert("DB 업데이트 완료 ");
            alert(res.data.result);
            if (confirm("테이블 재로딩 단계를 진행 하시겠습니까?")) {
              this.setNextStep();
              return;
            }
          } else {
            alert("업데이트 된 파일들이 없습니다.");
          }

          this.clearInputFileInfo();
        })
        .catch(e => {
          console.log(e);
        });
    },
  },
};
</script>
<style lang="scss">
html,
body {
  margin: 0px !important;
  padding: 0px !important;

  &::-webkit-scrollbar {
    width: 6px;
    background: none;
  }
  &::-webkit-scrollbar-thumb {
    background: #a8a8a8;
    border-radius: 10px;
  }
  &::-webkit-scrollbar-track {
    background: var(--dynamic-login-bg);
  }
}
.body-container {
  position: relative;
  font-family: "NotoSansKR";
  font-size: 0.875rem;
  // min-height: 100vh;
}
.content-container {
  display: flex;
  background-color: var(--dynamic-login-bg);
  min-height: calc(100vh - 58px);

  .router {
    flex: 5;
    padding: 12px 18px 24px;
    min-width: 600px;
  }
}
</style>
