<template>
  <div>
    <div class="group-list">
      <!-- 서버 시간 변경  -->
      <h2>Server DateTime Change</h2>
      <div class="page-result">
        <div class="panel">
          <div class="panel-body">
            <div class="panel-table">
              <div v-if="changeDatetimeServerList.length == 0">
                <label>시간 변경이 가능한 서버가 없습니다.</label>
              </div>
              <div v-if="changeDatetimeServerList.length != 0">
                <h3>시간 변경을 진행할 서버를 선택해주세요</h3>
                <table class="table table-bordered table-hover dataTable">
                  <thead class="table-align">
                    <tr>
                      <th>타겟서버</th>
                      <th>선택된 서버의 현재 시간</th>
                      <th>변경 시간</th>
                    </tr>
                  </thead>
                  <tbody class="table-align">
                    <tr>
                      <td>
                        <select
                          v-model="selectServerName"
                          class="form-control"
                          @change="getTargetServerDt($event.target.value)">
                          <option
                            v-for="serverInfo in changeDatetimeServerList"
                            :key="serverInfo.id"
                            :value="serverInfo.name">
                            {{ serverInfo.name }}
                          </option>
                        </select>
                      </td>
                      <td>
                        <locale-date-time :value="expectedServerDt" />
                      </td>
                      <td>
                        <flat-pickr
                          v-model="setServerDt"
                          class="form-control"
                          :config="dateTimeConfig"
                          placeholder="변경 될 시간 선택"></flat-pickr>
                      </td>
                    </tr>
                  </tbody>
                </table>
                <td>
                  <button type="submit" @click="resetServerDt()">리셋</button>
                </td>
                <td v-if="setServerDt != ''">
                  <button type="submit" @click="changeServerDt()">적용</button>
                </td>
              </div>
            </div>
          </div>
        </div>
      </div>
      <!-- 서버 시간 변경 종료 -->

      <!--File MD5 비교 시작  -->
      <h2>테이블 File 비교</h2>
      <div class="page-result">
        <div class="panel">
          <div class="panel-body">
            <div class="panel-table">
              <h3>비교할 DB 이름을 선택해주세요</h3>
              <table class="table table-bordered table-hover dataTable">
                <thead class="table-align">
                  <tr>
                    <th>DB 이름</th>
                    <th>Client SQLITE DB 선택</th>
                    <th></th>
                  </tr>
                </thead>
                <tbody class="table-align">
                  <tr>
                    <td>
                      <select v-model="selectDbName" class="form-control">
                        <option v-for="dbInfo in dbList" :key="dbInfo.id">
                          {{ dbInfo.name }}
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
                          accept=".txt"
                          @change="ReadSqliteFile" />
                      </form>
                    </td>
                    <td v-if="clientSqliteFile != null">
                      <div v-if="compareResultList == null">
                        <button type="submit" @click="CompareFileMD5()">실행</button>
                      </div>
                      <div v-else>
                        <button type="submit" @click="ShowCompareResultModal()">
                          조회 결과 확인
                        </button>
                        <button type="submit" @click="ClearCompareResult()">초기화</button>
                      </div>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
      <!--File MD5 비교 종료 -->
    </div>
  </div>
  <!-- 서버 생성 모달 START -->
  <modal-view :open-modal="showModal" :title="modalTitle" @close-modal="CloseCompareResultModal">
    <template #default>
      <div class="modals-body modals-vertical-body">
        <table class="table">
          <thead class="table-align">
            <tr>
              <th>파일 이름</th>
              <th>현재 MD5</th>
              <th>UpdateDt (클라이언트 기준)</th>
              <th>서버에서 사용 유무</th>
              <th>비교 결과</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(result, index) in compareResultList" :key="index">
              <td>{{ result.Name }}</td>
              <td>{{ result.MD5 }}</td>
              <td>{{ result.UpdateDt }}</td>
              <td v-if="result.isUsingTableInServer == true">사용</td>
              <td v-if="result.isUsingTableInServer == false" style="background-color: yellow">
                미사용
              </td>
              <td v-if="result.isSameMD5 == true" style="background-color: forestgreen">일치</td>
              <td v-if="result.isSameMD5 == false" style="background-color: crimson">불일치</td>
            </tr>
          </tbody>
        </table>
      </div>
    </template>

    <template #footer>
      <button type="button" class="button button-close" @click="CloseCompareResultModal">
        닫기
      </button>
    </template>
  </modal-view>
  <!-- 서버 생성 모달 END -->
</template>

<script>
import http from "@/utils/http-common";
import LocaleDateTime from "@/components/LocaleDateTime";
import ModalView from "@/components/ModalView";
// <!-- flatpickr -->
import flatPickr from "vue-flatpickr-component";
import "flatpickr/dist/flatpickr.css";
import ConfirmDatePlugin from "flatpickr/dist/plugins/confirmDate/confirmDate.js";
export default {
  name: "TheTools",
  components: {
    flatPickr,
    LocaleDateTime,
    ModalView,
  },
  data() {
    return {
      selectServerName: "",
      selectDbName: "",
      setServerDt: "",
      changeDatetimeServerList: [],
      dbList: [],
      intervalId: null,
      currentServerDt: new Date(),
      expectedServerDt: new Date(),
      dateTimeConfig: {
        enableTime: true,
        wrap: true,
        altFormat: "Y-m-d H:i:S",
        altInput: true,
        dateFormat: "Y-m-d H:i:S",
        plugins: [new ConfirmDatePlugin()],
        time_24hr: true,
      },

      compareResultList: null,
      clientSqliteFile: null,
      selectServerInfo: null,
      showModal: false,
      modalTitle: "File MD5 비교 결과",
    };
  },
  async mounted() {
    await this.loadServerList();
  },
  beforeUnmount() {
    clearInterval(this.intervalId);
  },
  methods: {
    StartClock() {
      if (this.intervalId) clearInterval(this.intervalId);
      this.expectedServerDt = new Date(this.currentServerDt.getTime());
      this.intervalId = setInterval(() => {
        //const now = new Date();
        //const elapsed = now - this.currentServerDt; // ms 단위

        // 서버 기준 시간 + 경과 시간
        this.expectedServerDt = new Date(this.expectedServerDt.getTime() + 1000);
      }, 1000);
    },
    async ClearCompareResult() {
      this.compareResultList = null;
      this.selectDbName = "";
      this.clientSqliteFile = null;
      document.getElementById("selectFile").value = "";
    },
    async ShowCompareResultModal() {
      this.showModal = true;
    },
    async CloseCompareResultModal() {
      this.showModal = false;
    },
    async CompareFileMD5() {
      const formData = new FormData();
      for (let index = 0; index < this.clientSqliteFile.length; ++index) {
        formData.append("files", this.clientSqliteFile[index]);
      }
      formData.append("dbName", this.selectDbName);

      http
        .post("/compare-file-md5", formData, {
          headers: {
            "Content-Type": "multipart/form-data",
          },
        })
        .then(res => {
          this.compareResultList = res.data.rows;
          console.log(res.data.rows);
        })
        .catch(err => {
          console.log(err);
        });
    },
    async ReadSqliteFile($event) {
      this.clientSqliteFile = $event.target.files;
    },
    async resetServerDt() {
      if (!confirm("리셋을 진행 하시겠습니까?")) {
        return;
      }

      http
        .post("/reset-server-dt", {
          serverName: this.selectServerName,
        })
        .then(res => {
          alert(`실행결과 서버 시간 : ${res.data.result}`);
          this.getTargetServerDt(this.selectServerName);
        })
        .catch(err => {
          console.log(err);
        });
    },
    async changeServerDt() {
      if (!confirm("시간 변경을 진행 하시겠습니까?")) {
        return;
      }

      http
        .post("/set-server-dt", {
          serverName: this.selectServerName,
          serverDt: this.setServerDt,
        })
        .then(res => {
          alert(`실행결과 서버 시간 : ${res.data.result}`);
          this.getTargetServerDt(this.selectServerName);
          this.StartClock();
        })
        .catch(err => {
          console.log(err);
        });
    },
    async getTargetServerDt(serverName) {
      http
        .get(`/server-dt/${serverName}`)
        .then(res => {
          this.currentServerDt = new Date(res.data);
          this.StartClock();
        })
        .catch(err => {
          console.log(err);
        });
    },
    async loadServerList() {
      http
        .get("/all")
        .then(res => {
          if (res.data.serverList.length != 0) {
            this.dbList = res.data.dbList;
            for (let serverInfo of res.data.serverList) {
              if (serverInfo.is_possible_change_server_dt != 0 && serverInfo.status == "RUNNING") {
                this.changeDatetimeServerList.push(serverInfo);
              }
            }
            if (this.changeDatetimeServerList.length != 0) {
              this.selectServerName = this.changeDatetimeServerList[0].name;
              this.getTargetServerDt(this.changeDatetimeServerList[0].name);
            }
          }
        })
        .catch(err => {
          console.log(err);
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
