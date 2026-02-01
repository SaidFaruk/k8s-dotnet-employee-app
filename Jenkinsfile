pipeline {
  agent any

  environment {
    K3S_HOST = "100.52.195.148"
    K3S_USER = "ec2-user"
    K3S_CRED = "k3s-ssh"

    REPO_URL  = "https://github.com/SaidFaruk/k8s-dotnet-employee-app.git"
    REMOTE_DIR = "/home/ec2-user/k8s-dotnet-employee-app"
    K8S_BASE   = "k8s/base"
  }

  options { timestamps() }

  stages {

    stage("Checkout") {
      steps {
        checkout scm
      }
      post {
        success {
          input message: "Repo pulled. Want me to continue?", ok: "Yep, go on"
        }
      }
    }

    stage("Prepare SSH known_hosts") {
      steps {
        sh """
          mkdir -p ~/.ssh
          chmod 700 ~/.ssh
          ssh-keyscan -H ${K3S_HOST} >> ~/.ssh/known_hosts 2>/dev/null || true
        """
      }
      post {
        success {
          input message: "SSH host key is saved. Continue?", ok: "Let's do it"
        }
      }
    }

    stage("SSH test to k3s server") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh -o StrictHostKeyChecking=yes ${K3S_USER}@${K3S_HOST} 'whoami && hostname && sudo k3s kubectl get nodes'
          """
        }
      }
      post {
        success {
          input message: "SSH looks good and k3s is reachable. Continue?", ok: "Go"
        }
      }
    }

    stage("Sync repo on server (clone/pull)") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e

              # Hangi branch deploy edilecek? (Multibranch ise BRANCH_NAME gelir; yoksa main)
              BR="\${BRANCH_NAME:-main}"

              if [ ! -d "${REMOTE_DIR}/.git" ]; then
                rm -rf "${REMOTE_DIR}"
                git clone "${REPO_URL}" "${REMOTE_DIR}"
              fi

              cd "${REMOTE_DIR}"
              git fetch --all
              git checkout "\$BR" || git checkout -b "\$BR" "origin/\$BR"
              git reset --hard "origin/\$BR"
              git rev-parse --short HEAD
            '
          """
        }
      }
      post {
        success {
          input message: "Repo is synced on the server. Continue?", ok: "Sure"
        }
      }
    }

    stage("Apply namespace.yaml") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e
              cd "${REMOTE_DIR}"
              sudo k3s kubectl apply -f "${K8S_BASE}/namespace.yaml"
              sudo k3s kubectl get ns | tail -n +1
            '
          """
        }
      }
      post {
        success {
          input message: "Namespace is applied. Should I move on?", ok: "Yes"
        }
      }
    }

    stage("Apply Postgres") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e
              cd "${REMOTE_DIR}"
              sudo k3s kubectl apply -f "${K8S_BASE}/postgres"
              echo "Postgres pods (if any):"
              sudo k3s kubectl get pods -A -o wide | grep -i postgres || true
            '
          """
        }
      }
      post {
        success {
          input message: "Postgres is applied. Continue?", ok: "Yep"
        }
      }
    }

    stage("Run migration job (api/00*)") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e
              cd "${REMOTE_DIR}"

              MIG_FILE=\$(ls -1 "${K8S_BASE}/api"/00*.y*ml 2>/dev/null | head -n 1 || true)
              if [ -n "\$MIG_FILE" ]; then
                echo "Migration manifest: \$MIG_FILE"
                sudo k3s kubectl apply -f "\$MIG_FILE"
              else
                echo "No 00* migration manifest found under ${K8S_BASE}/api. Skipping."
              fi
            '
          """
        }
      }
      post {
        success {
          input message: "Migration step is done. Want to continue?", ok: "Continue"
        }
      }
    }

    stage("Apply API (everything in api/)") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e
              cd "${REMOTE_DIR}"
              sudo k3s kubectl apply -f "${K8S_BASE}/api"
              echo "API deployments (if any):"
              sudo k3s kubectl get deploy -A | grep -i api || true
            '
          """
        }
      }
      post {
        success {
          input message: "API manifests are applied. Keep going?", ok: "Go on"
        }
      }
    }

    stage("Apply Frontend") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e
              cd "${REMOTE_DIR}"
              sudo k3s kubectl apply -f "${K8S_BASE}/frontend"
              echo "Frontend deployments (if any):"
              sudo k3s kubectl get deploy -A | grep -i frontend || true
            '
          """
        }
      }
      post {
        success {
          input message: "Frontend is applied. Continue?", ok: "Yep"
        }
      }
    }

    stage("Apply Ingress") {
      steps {
        sshagent(credentials: ["${K3S_CRED}"]) {
          sh """
            ssh ${K3S_USER}@${K3S_HOST} '
              set -e
              cd "${REMOTE_DIR}"
              sudo k3s kubectl apply -f "${K8S_BASE}/ingress"
              sudo k3s kubectl get ingress -A || true
            '
          """
        }
      }
      post {
        success {
          input message: "Ingress is applied. Continue?", ok: "Sure"
        }
      }
    }

stage("Port-forward frontend (8080:80)") {
  steps {
    sshagent(credentials: ["${K3S_CRED}"]) {
      sh """
        ssh ${K3S_USER}@${K3S_HOST} '
          set -e

          NS="k8sdotnetapp"
          echo "Using namespace: \$NS"

          # 8080 doluysa yeni port-forward başlatmayalım
          if ss -lnt | grep -q ":8080"; then
            echo "Port 8080 is already in use on the server. Skipping port-forward."
            exit 0
          fi

          nohup k3s kubectl -n "\$NS" port-forward --address 0.0.0.0 svc/frontend 8080:80 \
            > /tmp/portforward-8080.log 2>&1 &

          sleep 2
          echo "Port-forward started. Last logs:"
          tail -n 30 /tmp/portforward-8080.log || true
        '
      """
    }
  }
}
  }
}
