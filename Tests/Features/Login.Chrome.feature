@chrome
Feature: Login Chrome

  Scenario Outline: UC1 Login with empty credentials
    Given I am on the login page
    When I enter "<username>" and "<password>"
    And I clear all credentials
    And I click login
    Then I should see error containing "Username is required"

    Examples:
      | username | password |
      | ddd      | eee      |

  Scenario Outline: UC2 Login with username only
    Given I am on the login page
    When I enter "<username>" and "<password>"
    And I clear the password
    And I click login
    Then I should see error containing "Password is required"

    Examples:
      | username | password |
      | ddd      | eee      |

  Scenario Outline: UC3 Login with valid credentials
    Given I am on the login page
    When I enter "<username>" and "<password>"
    And I click login
    Then I should be redirected to inventory page

    Examples:
      | username      | password     |
      | standard_user | secret_sauce |