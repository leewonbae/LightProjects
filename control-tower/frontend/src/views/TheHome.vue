<template>
  <div>
    <div class="group-list">
      <h2>서버 리스트</h2>

      <div class="page-result">
        <div class="panel">
          <div class="panel-body">
            <div class="panel-table">
              <table class="table table-bordered table-hover dataTable">
                <thead class="table-align">
                  <tr>
                    <th>서버이름</th>
                    <th>접속정보</th>
                    <th>브랜치</th>
                    <th>DB명</th>
                    <th>상태</th>
                    <th>서버 구동 날짜</th>
                    <th>관리</th>
                  </tr>
                </thead>
                <tbody class="table-align">
                  <tr v-for="server in serverList" :key="server.id">
                    <td>{{ server.name }}</td>
                    <td>{{ server.ip }}:{{ server.port }}</td>
                    <td>{{ server.branch }}</td>
                    <td>
                      {{ server.database }}
                    </td>
                    <td>{{ server.status }}</td>
                    <td><locale-date-time :value="server.reg_dt" /></td>
                    <td>
                      <button
                        v-if="server.name !== 'ta-dev'"
                        type="button"
                        class="button orange"
                        @click="reloadTable(server)">
                        테이블 재로딩
                      </button>
                      <button
                        v-if="server.name !== 'ta-dev'"
                        type="button"
                        class="button red"
                        @click="deleteServer(server.name)">
                        삭제
                      </button>
                      <button
                        v-if="server.name === 'ta-dev'"
                        type="button"
                        class="button green"
                        @click="updateServer(server.id, server.name)">
                        업데이트
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="new">
          <button type="button" class="button blue" @click="openNewServerModal">
            신규 서버 생성
          </button>
        </div>
      </div>

      <!-- 서버 생성 모달 START -->
      <modal-view
        :open-modal="showNewServerModal"
        :title="serverModalTitle"
        @close-modal="closeNewServerModal">
        <template #default>
          <div class="modals-body modals-vertical-body">
            <table class="table">
              <tr class="table-tr">
                <th>서버 이름</th>
                <td>
                  <input v-model="newServer.name" placeholder="서버명 입력해주세요." type="text" />
                </td>
              </tr>
              <tr class="table-tr">
                <th>브랜치명</th>
                <td>
                  <input
                    v-model="newServer.branch"
                    placeholder="서버에 적용할 브랜치명 입력해주세요. (develop,release,master,feature/*)"
                    type="text" />
                </td>
              </tr>
              <tr class="table-tr">
                <th>DB 선택</th>
                <td>
                  <span class="red-color">
                    신규 DB 생성이 필요하면 아래 신규 DB 생성 후에 선택해주세요.
                  </span>
                  <br />
                  <select v-model="newServer.database" class="form-control">
                    <option v-for="db in dbList" :key="db.id" :value="db.name">
                      {{ db.name }}
                    </option>
                  </select>
                </td>
              </tr>
              <tr class="table-tr">
                <th>시간 변경 가능 여부</th>
                <td>
                  <select v-model="newServer.isPossibleChangeServerDt" class="form-control">
                    <option :value="true">가능</option>
                    <option :value="false">불가능</option>
                  </select>
                </td>
              </tr>
            </table>
          </div>
        </template>

        <template #footer>
          <button type="button" class="button" @click="addNewServer">생성</button>
          <button type="button" class="button button-close" @click="closeNewServerModal">
            닫기
          </button>
        </template>
      </modal-view>
      <!-- 서버 생성 모달 END -->
    </div>

    <div class="group-list">
      <h2>DB 리스트</h2>

      <div class="page-result">
        <div class="panel">
          <div class="panel-body">
            <div class="panel-table">
              <table class="table table-bordered table-hover dataTable">
                <thead class="table-align">
                  <tr>
                    <th>DB명</th>
                    <th>브랜치</th>
                    <th>관리</th>
                  </tr>
                </thead>
                <tbody class="table-align">
                  <tr v-for="db in dbList" :key="db.id">
                    <td>{{ db.name }}</td>
                    <td>{{ db.branch }}</td>
                    <td>
                      <button
                        v-if="db.name !== 'default'"
                        type="button"
                        class="button orange"
                        @click="updateDataTableDb(db)">
                        데이터 테이블 업뎃
                      </button>
                      <button
                        v-if="db.name !== 'default'"
                        type="button"
                        class="button red"
                        @click="deleteDb(db.name)">
                        삭제
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>

        <div class="new">
          <button type="button" class="button blue" @click="openNewDbModal">신규 DB 생성</button>
        </div>
      </div>

      <!-- 서버 생성 모달 START -->
      <modal-view :open-modal="showNewDbModal" :title="dbModalTitle" @close-modal="closeNewDbModal">
        <template #default>
          <div class="modals-body modals-vertical-body">
            <table class="table">
              <tr class="table-tr">
                <th>DB명</th>
                <td>
                  <input v-model="newDb.name" placeholder="DB명 입력해주세요." type="text" />
                </td>
              </tr>
              <tr class="table-tr">
                <th>브랜치명</th>
                <td>
                  <input
                    v-model="newDb.branch"
                    placeholder="DB에 적용할 브랜치명 입력해주세요. (develop,release,master,feature/*)"
                    type="text" />
                </td>
              </tr>
            </table>
          </div>
        </template>

        <template #footer>
          <button type="button" class="button" @click="addNewDb">생성</button>
          <button type="button" class="button button-close" @click="closeNewDbModal">닫기</button>
        </template>
      </modal-view>
      <!-- 서버 생성 모달 END -->
    </div>

    <div class="group-list">
      <h2>Docker 리스트</h2>

      <div class="page-result">
        <div class="panel">
          <div class="panel-body">
            <div class="panel-table">
              <table class="table table-bordered table-hover dataTable">
                <thead class="table-align">
                  <tr>
                    <th>Docker명</th>
                    <th>이미지ID</th>
                    <th>관리</th>
                  </tr>
                </thead>
                <tbody class="table-align">
                  <tr v-for="docker in dockerList" :key="docker.id">
                    <td>{{ docker.name }}</td>
                    <td>{{ docker.image_id }}</td>
                    <td>
                      <button type="button" class="button red" @click="deleteDocker(docker)">
                        삭제
                      </button>
                    </td>
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import http from "@/utils/http-common";
import LocaleDateTime from "@/components/LocaleDateTime";
import ModalView from "@/components/ModalView";

export default {
  name: "TheHome",
  components: {
    LocaleDateTime,
    ModalView,
  },
  data() {
    return {
      serverList: [],
      serverImageMap: {},
      serverDatabaseMap: {},
      showNewServerModal: false,
      newServer: {
        name: "",
        branch: "",
        database: "default",
        isPossibleChangeServerDt: false,
      },
      serverModalTitle: "서버 생성",

      dbList: [],
      dbMap: {},
      showNewDbModal: false,
      newDb: {
        name: "",
        branch: "",
      },
      dbModalTitle: "DB 생성",

      dockerList: [],
      dockerMap: {},
    };
  },
  async mounted() {
    await this.loadServerList();
  },
  methods: {
    async loadServerList() {
      http
        .get("/all")
        .then(res => {
          if (res.data) {
            this.serverList = res.data.serverList;
            this.serverImageMap = Object.fromEntries(
              this.serverList.filter(e => e.image_id != null).map(e => [e.image_id, e]),
            );
            this.serverDatabaseMap = Object.fromEntries(
              this.serverList.filter(e => e.database != null).map(e => [e.database, e]),
            );

            this.dbList = res.data.dbList;
            this.dbMap = Object.fromEntries(this.dbList.map(e => [e.id, e]));

            this.dockerList = res.data.dockerList;
            this.dockerMap = Object.fromEntries(this.dockerList.map(e => [e.id, e]));
          }
        })
        .catch(err => {
          console.log(err);
        });
    },
    openNewServerModal() {
      this.showNewServerModal = true;
      this.newServer = {
        name: "",
        branch: "",
        database: "default",
        isPossibleChangeServerDt: false,
      };
    },
    closeNewServerModal() {
      this.showNewServerModal = false;
    },
    async addNewServer() {
      if (!this.newServer.name || !this.newServer.branch) {
        return;
      }

      if (!confirm(this.newServer.name + "을 생성하시겠습니까?")) {
        return;
      }

      http
        .post("/add", this.newServer)
        .then(res => {
          if (res.data.name === this.newServer.name) {
            this.loadServerList();
          }
          this.closeNewServerModal();
        })
        .catch(err => {
          console.log(err);
          this.closeNewServerModal();
        });
    },
    async updateServer(serverId, serverName) {
      if (confirm(serverName + "를 업데이트 진행하시겠습니까?")) {
        http
          .post("/update-server", {
            id: serverId,
          })
          .then(res => {
            if (res.data.id === serverId) {
              this.loadServerList();
            }
          });
      }
    },
    async deleteServer(serverName) {
      if (confirm(`서버 [${serverName}]를 삭제하시겠습니까?`)) {
        http
          .post("/delete-server", {
            name: serverName,
          })
          .then(res => {
            if (res.data.name === serverName) {
              this.loadServerList();
            }
          });
      }
    },
    async reloadTable(server) {
      if (!confirm(`서버 [${server.name}]의 테이블을 재로딩하시겠습니까?`)) {
        return;
      }
      http.get(`http://${server.ip}:${server.port}/system/reload-table`).then(res => {
        if (res.data) {
          this.loadServerList();
        }
      });
    },
    openNewDbModal() {
      this.showNewDbModal = true;
      this.newDb = {
        name: "",
        branch: "",
      };
    },
    closeNewDbModal() {
      this.showNewDbModal = false;
    },

    async addNewDb() {
      if (!this.newDb.name || !this.newDb.branch) {
        return;
      }

      if (!confirm(this.newDb.name + "을 생성하시겠습니까?")) {
        return;
      }

      http
        .post("/add-db", this.newDb)
        .then(res => {
          if (res.data.name === this.newDb.name) {
            this.serverList();
          }
          this.closeNewDbModal();
        })
        .catch(err => {
          console.log(err);
          this.closeNewDbModal();
        });
    },

    async updateDataTableDb(db) {
      if (!confirm(db.name + "의 데이터 테이블을 업뎃하시겠습니까?")) {
        return;
      }

      http
        .post("/update-data-table", db)
        .then(res => {
          if (res.data.name === db.name) {
            this.serverList();
          }
        })
        .catch(err => {
          console.log(err);
        });
    },

    async deleteDb(dbName) {
      if (!confirm(`DB [${dbName}]를 삭제하시겠습니까?`)) {
        return;
      }

      if (this.serverDatabaseMap[dbName]) {
        alert(`해당 DB를 사용하고 있는 서버[${this.serverDatabaseMap[dbName].name}]가 있습니다.`);
        return;
      }

      http.delete(`/delete-db/${dbName}`).then(res => {
        if (res.data.name === dbName) {
          this.loadServerList();
        }
      });
    },

    async deleteDocker(docker) {
      if (!confirm(`Docker [${docker.name}]를 삭제하시겠습니까?`)) {
        return;
      }

      if (this.serverImageMap[docker.image_id]) {
        alert(
          `해당 이미지를 사용하고 있는 서버[${
            this.serverImageMap[docker.image_id].name
          }]가 있습니다.`,
        );
        return;
      }

      http.delete(`/docker-image/${docker.name}`).then(res => {
        if (res.data.name === docker.name) {
          this.loadServerList();
        }
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
